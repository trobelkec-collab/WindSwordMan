const WebSocket = require('ws');

const ws = new WebSocket('ws://127.0.0.1:8090');

ws.on('open', function open() {
    console.log('Connected to Unity MCP');

    // JSON-RPC 2.0 request to list tools
    const request = {
        jsonrpc: '2.0',
        id: 1,
        method: 'tools/list',
        params: {}
    };

    ws.send(JSON.stringify(request));
});

ws.on('message', function incoming(data) {
    const response = JSON.parse(data);
    console.log('Received response:');
    console.log(JSON.stringify(response, null, 2));
    ws.close();
});

ws.on('error', function error(err) {
    console.error('Connection error:', err);
});
