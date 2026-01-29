
const WebSocket = require('ws');

const ws = new WebSocket('ws://localhost:8090/McpUnity');

ws.on('open', function open() {
    console.log('Connected to McpUnity');

    const request = {
        jsonrpc: '2.0',
        method: 'execute_menu_item',
        params: {
            menuPath: 'Tools/Edo Highway/Setup Phase 1'
        },
        id: 1
    };

    console.log('Triggering Setup Phase 1...');
    ws.send(JSON.stringify(request));
});

ws.on('message', function message(data) {
    const response = JSON.parse(data);
    console.log('Received:', response);
    ws.close();
    process.exit(0);
});

ws.on('error', function error(err) {
    console.error('WebSocket error:', err);
    process.exit(1);
});
