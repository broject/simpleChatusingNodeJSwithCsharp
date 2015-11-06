net = require('net');
helpers = require('./helpers');
var id2socket = new Object;
var socket2id = new Object;

console.log('Messager server starting...');
var sockets = [];
var PORT = 6891;
var server = net.Server(function (socket) {
    //socket = new JsonSocket(socket);
    sockets.push(socket);
    
    socket.on('connect', function (conn) {
        console.log('Connection from ' + socket.remoteAddress + ':' + socket.remotePort);
    });
    
    socket.on('data', function (model) {
        console.log('Received data: ' + model);
        try {
            var jobj = JSON.parse(model);
            
            if (jobj.model == 'UserAccount' && jobj.username !== undefined) {
                id2socket[jobj.username] = socket;
                socket2id[socket] = jobj.username;
                
                for (var i = 0; i < sockets.length; i++) {
                    if (sockets[i] == socket) {
                        jobj.role = 1;
                        sockets[i].write(helpers.to_jstr(jobj));
                        
                        console.log('UID: ' + jobj.username + ' with role [' + jobj.role + ']');
                    } else {
                        var message = { type: 22, data: jobj.username + ' user connected.' };
                        sockets[i].write(helpers.to_jstr(message));
                    }
                }
            } else {
                var jstr = helpers.to_jstr(jobj);
                for (var i = 0; i < sockets.length; i++) {
                    //if (sockets[i] == socket) // don't send the message to yourself
                    //    continue;
                    sockets[i].write(jstr);
                }
            }
        } catch (SyntaxError) {
            console.log('Invalid JSON: ' + model);
            socket.write('{"success":"false","response":"invalid JSON"}');
        }
    });
    
    socket.on('end', function () {
        username = socket2id[socket];
        console.log('Disconnect by [' + username + ']');
        console.log('Removing from map socket: ' + socket + ', username: ' + username);

        delete id2socket[username];
        delete socket2id[socket];
        
        var i = sockets.indexOf(socket);
        sockets.splice(i, 1);
    });
    
    socket.on('timeout', function () {
        console.log('Server timeout');
    });

});
server.on('error', function (e) {
    if (e.code == 'EADDRINUSE' || e.code == 'ECONNRESET') {
        console.log('Address in use, retrying...');
        setTimeout(function () {
            server.close();
            server.listen(PORT, HOST);
            console.log('System reloaded.');
        }, 1000);
    }
});
server.listen(PORT);
console.log('Messager server started.');