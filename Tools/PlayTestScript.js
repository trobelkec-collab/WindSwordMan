
const WebSocket = require('ws');

const ws = new WebSocket('ws://localhost:8090/McpUnity');
let step = 0;

ws.on('open', function open() {
    console.log('Connected to McpUnity');

    // Step 1: Execute Menu Item
    const request = {
        jsonrpc: '2.0',
        method: 'execute_menu_item',
        params: {
            menuPath: 'Tools/Edo Highway/Play Test'
        },
        id: 1
    };

    console.log('Sending Execute Menu Item request...');
    ws.send(JSON.stringify(request));
});

ws.on('message', function message(data) {
    const response = JSON.parse(data);
    // console.log('Received:', response);

    if (response.id === 1 || response.id === '1') { // MCP might return string ID
        if (response.result && response.result.success) {
            console.log("Menu item executed successfully (" + response.result.message + "). Waiting for Play Mode...");
            setTimeout(() => {
                // Step 2: Get Console Logs
                const logRequest = {
                    jsonrpc: '2.0',
                    method: 'get_console_logs',
                    params: {},
                    id: 2
                };
                console.log('Fetching logs...');
                ws.send(JSON.stringify(logRequest));
            }, 3000);
        } else {
            console.error("Failed to execute menu item:", response);
            // Even if failed, try to get logs to see why
            setTimeout(() => {
                const logRequest = {
                    jsonrpc: '2.0',
                    method: 'get_console_logs',
                    params: {},
                    id: 2
                };
                ws.send(JSON.stringify(logRequest));
            }, 1000);
        }
    } else if (response.id === 2 || response.id === '2') {
        console.log("Logs Received:");
        if (response.result) {
            // The result might be the logs directly or wrapped
            // ConsoleLogsService returns a list of log entries
            // Let's inspect the structure
            console.log(JSON.stringify(response.result, null, 2));
        }
        ws.close();
        process.exit(0);
    }
});

ws.on('error', function error(err) {
    console.error('WebSocket error:', err);
    process.exit(1);
});
