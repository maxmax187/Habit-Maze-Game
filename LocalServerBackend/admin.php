<?php
// ============================================================
// CONFIGURATION
// ============================================================
require_once __DIR__ . '/env.php';
loadEnv();
// ============================================================

// // Load .env credentials (same pattern as index.php)
// $envFile = __DIR__ . '/.env';
// if (file_exists($envFile)) {
//     $env = parse_ini_file($envFile);
//     foreach ($env as $key => $value) {
//         putenv("$key=$value");
//     }
// }

session_start();

$exportPassword = getenv('EXPORT_PASSWORD') ?: 'NOT_CONFIGURED';
$error   = '';
$message = '';
$results = null;
$columns = [];
$affected = null;

// Handle login
if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['password'])) {
    if ($_POST['password'] === getenv('EXPORT_PASSWORD')) {
        $_SESSION['admin_auth'] = true;
    } else {
        $error = 'Incorrect password.';
    }
}

// Handle logout
if (isset($_GET['logout'])) {
    session_destroy();
    header('Location: admin.php');
    exit;
}

$authed = isset($_SESSION['admin_auth']) && $_SESSION['admin_auth'] === true;

// Preset queries
$presets = [
    'show_tables'    => 'SHOW TABLES',
    'all_participants' => 'SELECT * FROM participants ORDER BY email',
    'all_rounds'     => 'SELECT * FROM rounds ORDER BY participantEmail, day, round',
    'all_logs'       => 'SELECT r.participantEmail, r.day, r.round, rl.t, rl.d FROM roundLogs rl JOIN rounds r ON r.id = rl.roundId ORDER BY r.participantEmail, r.day, r.round, rl.t LIMIT 500',
    'all_surveys'    => 'SELECT * FROM habitSurvey ORDER BY participantEmail, day',
    'clear_logs'     => 'TRUNCATE TABLE roundLogs',
    'clear_rounds'   => 'TRUNCATE TABLE rounds',
    'clear_participants' => 'TRUNCATE TABLE participants',
    'clear_surveys'  => 'TRUNCATE TABLE habitSurvey',
    'clear_all'      => 'TRUNCATE TABLE roundLogs; TRUNCATE TABLE habitSurvey; TRUNCATE TABLE rounds; TRUNCATE TABLE participants',
];

$preset_labels = [
    'show_tables'        => 'SHOW TABLES',
    'all_participants'   => 'All participants',
    'all_rounds'         => 'All rounds',
    'all_logs'           => 'All round logs (limit 500)',
    'all_surveys'        => 'All habit surveys',
    'clear_logs'         => '⚠ Clear round logs',
    'clear_rounds'       => '⚠ Clear rounds',
    'clear_participants' => '⚠ Clear participants',
    'clear_surveys'      => '⚠ Clear surveys',
    'clear_all'          => '⚠⚠ Clear ALL tables',
];

// Handle query execution
if ($authed && $_SERVER['REQUEST_METHOD'] === 'POST' && (isset($_POST['run_query']) || isset($_POST['preset']))) {
    $db = new mysqli(
        getenv('DB_HOST') ?: 'localhost',
        getenv('DB_USER'),
        getenv('DB_PASSWORD'),
        getenv('DB_NAME'),
        getenv('DB_PORT') ?: 3306
    );

    if ($db->connect_error) {
        $error = 'DB connection failed: ' . $db->connect_error;
    } else {
        $sql = '';

        if (isset($_POST['preset']) && isset($presets[$_POST['preset']])) {
            $sql = $presets[$_POST['preset']];
        } elseif (isset($_POST['query'])) {
            $sql = trim($_POST['query']);
        }

        if ($sql) {
            // Handle multi-statement queries (used for clear_all)
            if (strpos($sql, ';') !== false && substr_count($sql, ';') > 1) {
                $statements = array_filter(array_map('trim', explode(';', $sql)));
                $allOk = true;
                foreach ($statements as $stmt) {
                    if (!$db->query($stmt)) {
                        $error = 'Error: ' . $db->error;
                        $allOk = false;
                        break;
                    }
                }
                if ($allOk) $message = 'All statements executed successfully.';
            } else {
                $result = $db->query($sql);
                if ($result === false) {
                    $error = 'Query error: ' . $db->error;
                } elseif ($result === true) {
                    $affected = $db->affected_rows;
                    $message = 'Query executed. Rows affected: ' . $affected;
                } else {
                    $columns = array_column($result->fetch_fields(), 'name');
                    $results = $result->fetch_all(MYSQLI_ASSOC);
                    $message = count($results) . ' row(s) returned.';
                }
            }
        }
        $db->close();
    }
}
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>DB Admin</title>
    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Courier New', monospace;
            background: #0f0f0f;
            color: #e0e0e0;
            min-height: 100vh;
            padding: 2rem;
        }

        .login-wrap {
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 100vh;
            padding: 2rem;
        }

        .card {
            background: #1a1a1a;
            border: 1px solid #2a2a2a;
            padding: 2.5rem;
            width: 100%;
            max-width: 420px;
        }

        h1 {
            font-size: 0.75rem;
            letter-spacing: 0.2em;
            text-transform: uppercase;
            color: #666;
            margin-bottom: 2rem;
        }

        h1 span { color: #00ff88; }

        h2 {
            font-size: 0.7rem;
            letter-spacing: 0.15em;
            text-transform: uppercase;
            color: #555;
            margin-bottom: 0.75rem;
        }

        label {
            display: block;
            font-size: 0.7rem;
            letter-spacing: 0.15em;
            text-transform: uppercase;
            color: #666;
            margin-bottom: 0.5rem;
        }

        input[type="password"] {
            width: 100%;
            background: #0f0f0f;
            border: 1px solid #333;
            color: #e0e0e0;
            padding: 0.75rem 1rem;
            font-family: inherit;
            font-size: 0.9rem;
            outline: none;
            margin-bottom: 1rem;
        }

        input[type="password"]:focus { border-color: #00ff88; }

        textarea {
            width: 100%;
            background: #0f0f0f;
            border: 1px solid #333;
            color: #e0e0e0;
            padding: 0.75rem 1rem;
            font-family: inherit;
            font-size: 0.85rem;
            outline: none;
            resize: vertical;
            min-height: 100px;
            margin-bottom: 0.75rem;
        }

        textarea:focus { border-color: #00ff88; }

        .btn {
            background: #00ff88;
            color: #0f0f0f;
            border: none;
            padding: 0.65rem 1.25rem;
            font-family: inherit;
            font-size: 0.7rem;
            letter-spacing: 0.15em;
            text-transform: uppercase;
            cursor: pointer;
            font-weight: bold;
        }

        .btn:hover { background: #00cc6a; }

        .btn-danger {
            background: #1a1a1a;
            color: #ff4444;
            border: 1px solid #ff4444;
        }

        .btn-danger:hover { background: #ff4444; color: #0f0f0f; }

        .btn-ghost {
            background: #1a1a1a;
            color: #888;
            border: 1px solid #333;
        }

        .btn-ghost:hover { border-color: #888; color: #e0e0e0; }

        .error   { font-size: 0.8rem; color: #ff4444; padding: 0.75rem; background: #1a0000; border: 1px solid #440000; margin-bottom: 1rem; }
        .success { font-size: 0.8rem; color: #00ff88; padding: 0.75rem; background: #001a0d; border: 1px solid #004422; margin-bottom: 1rem; }

        .layout {
            display: grid;
            grid-template-columns: 260px 1fr;
            gap: 1.5rem;
            max-width: 1400px;
        }

        .sidebar { display: flex; flex-direction: column; gap: 1.5rem; }

        .panel {
            background: #1a1a1a;
            border: 1px solid #2a2a2a;
            padding: 1.25rem;
        }

        .preset-group { margin-bottom: 1rem; }
        .preset-group:last-child { margin-bottom: 0; }

        .preset-group-label {
            font-size: 0.6rem;
            letter-spacing: 0.2em;
            text-transform: uppercase;
            color: #444;
            margin-bottom: 0.5rem;
        }

        .preset-btn {
            display: block;
            width: 100%;
            text-align: left;
            background: none;
            border: none;
            color: #888;
            font-family: inherit;
            font-size: 0.78rem;
            padding: 0.35rem 0.5rem;
            cursor: pointer;
            letter-spacing: 0.02em;
        }

        .preset-btn:hover { color: #00ff88; background: #111; }

        .preset-btn.danger { color: #664444; }
        .preset-btn.danger:hover { color: #ff4444; }

        .results-panel { overflow-x: auto; }

        table {
            width: 100%;
            border-collapse: collapse;
            font-size: 0.78rem;
        }

        th {
            background: #111;
            color: #00ff88;
            text-align: left;
            padding: 0.5rem 0.75rem;
            font-size: 0.65rem;
            letter-spacing: 0.1em;
            text-transform: uppercase;
            white-space: nowrap;
            border-bottom: 1px solid #2a2a2a;
        }

        td {
            padding: 0.45rem 0.75rem;
            border-bottom: 1px solid #1f1f1f;
            color: #ccc;
            white-space: nowrap;
        }

        tr:hover td { background: #1f1f1f; }

        .topbar {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 1.5rem;
        }

        a.logout {
            font-size: 0.65rem;
            color: #444;
            text-decoration: none;
            letter-spacing: 0.1em;
            text-transform: uppercase;
        }

        a.logout:hover { color: #888; }

        .empty { color: #444; font-size: 0.8rem; padding: 1rem 0; }
    </style>
</head>
<body>

<?php if (!$authed): ?>
<div class="login-wrap">
    <div class="card">
        <h1>DB <span>Admin</span></h1>
        <?php if ($error): ?><div class="error"><?= htmlspecialchars($error) ?></div><?php endif; ?>
        <form method="POST">
            <label for="pw">Password</label>
            <input type="password" id="pw" name="password" autofocus>
            <button type="submit" class="btn">Authenticate</button>
        </form>
    </div>
</div>

<?php else: ?>

<div class="topbar">
    <h1 style="margin:0">DB <span style="color:#00ff88">Admin</span></h1>
    <a href="?logout" class="logout">Log out</a>
</div>

<div class="layout">
    <!-- Sidebar -->
    <div class="sidebar">
        <!-- Preset queries -->
        <div class="panel">
            <h2>Presets</h2>
            <div class="preset-group">
                <div class="preset-group-label">Inspect</div>
                <form method="POST">
                    <input type="hidden" name="preset" value="show_tables">
                    <button class="preset-btn" type="submit">Show tables</button>
                </form>
                <form method="POST">
                    <input type="hidden" name="preset" value="all_participants">
                    <button class="preset-btn" type="submit">All participants</button>
                </form>
                <form method="POST">
                    <input type="hidden" name="preset" value="all_rounds">
                    <button class="preset-btn" type="submit">All rounds</button>
                </form>
                <form method="POST">
                    <input type="hidden" name="preset" value="all_logs">
                    <button class="preset-btn" type="submit">All round logs (500)</button>
                </form>
                <form method="POST">
                    <input type="hidden" name="preset" value="all_surveys">
                    <button class="preset-btn" type="submit">All habit surveys</button>
                </form>
            </div>
            <div class="preset-group">
                <div class="preset-group-label">Danger zone</div>
                <form method="POST" onsubmit="return confirm('Clear roundLogs table?')">
                    <input type="hidden" name="preset" value="clear_logs">
                    <button class="preset-btn danger" type="submit">⚠ Clear round logs</button>
                </form>
                <form method="POST" onsubmit="return confirm('Clear rounds table?')">
                    <input type="hidden" name="preset" value="clear_rounds">
                    <button class="preset-btn danger" type="submit">⚠ Clear rounds</button>
                </form>
                <form method="POST" onsubmit="return confirm('Clear participants table?')">
                    <input type="hidden" name="preset" value="clear_participants">
                    <button class="preset-btn danger" type="submit">⚠ Clear participants</button>
                </form>
                <form method="POST" onsubmit="return confirm('Clear habitSurvey table?')">
                    <input type="hidden" name="preset" value="clear_surveys">
                    <button class="preset-btn danger" type="submit">⚠ Clear surveys</button>
                </form>
                <form method="POST" onsubmit="return confirm('CLEAR ALL TABLES? This cannot be undone.')">
                    <input type="hidden" name="preset" value="clear_all">
                    <button class="preset-btn danger" type="submit">⚠⚠ Clear ALL tables</button>
                </form>
            </div>
        </div>

        <!-- Custom query -->
        <div class="panel">
            <h2>Custom Query</h2>
            <form method="POST">
                <textarea name="query" placeholder="SELECT * FROM rounds LIMIT 10"><?= isset($_POST['query']) ? htmlspecialchars($_POST['query']) : '' ?></textarea>
                <button type="submit" name="run_query" value="1" class="btn">Run</button>
            </form>
        </div>
    </div>

    <!-- Results -->
    <div class="panel results-panel">
        <h2>Results</h2>
        <br>
        <?php if ($error): ?>
            <div class="error"><?= htmlspecialchars($error) ?></div>
        <?php elseif ($message): ?>
            <div class="success"><?= htmlspecialchars($message) ?></div>
        <?php endif; ?>

        <?php if ($results !== null && count($results) > 0): ?>
            <table>
                <thead>
                    <tr><?php foreach ($columns as $col): ?><th><?= htmlspecialchars($col) ?></th><?php endforeach; ?></tr>
                </thead>
                <tbody>
                    <?php foreach ($results as $row): ?>
                        <tr><?php foreach ($row as $val): ?><td><?= htmlspecialchars($val ?? 'NULL') ?></td><?php endforeach; ?></tr>
                    <?php endforeach; ?>
                </tbody>
            </table>
        <?php elseif ($results !== null): ?>
            <div class="empty">No rows returned.</div>
        <?php else: ?>
            <div class="empty">Run a query or select a preset to see results here.</div>
        <?php endif; ?>
    </div>
</div>

<?php endif; ?>
</body>
</html>