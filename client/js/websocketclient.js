/* Class derived from "Alchemy Websockets Client Library"
Distributed under LGPL
https://github.com/Olivine-Labs/Alchemy-Websockets-Client-Library
*/
		
(function() {
  function WebSocketClient(options) {

    if (!this instanceof WebSocketClient) {
      return new WebSocketClient(options);
    } else {
      if (!options) {
        options = {};
      }

      this._options = MergeDefaults(this._defaultOptions, options);

      if (!window.WebSocket) {
        throw 'UNSUPPORTED: Websockets are not supported in this browser!';
      }

      this.SocketState = WebSocketClient.prototype.SocketStates.Closed;

      this.Connected = this._options.Connected;
      this.Disconnected = this._options.Disconnected;
      this.MessageReceived = this._options.MessageReceived;
    }
  }

  WebSocketClient.prototype = {
    _socket: {},
    _lastReceive: (new Date()).getTime(),
    _options: {},

    SocketStates: {
      Connecting: 0,
      Open: 1,
      Closing: 2,
      Closed: 3
    },

    SocketState: 3,

    Start: function() {
      var server = 'ws://' + this._options.Server + ':' + this._options.Port + '/' + this._options.Action;
      var ACInstance = this;
      this._socket = new WebSocket(server);
      this._socket.onopen = function() { ACInstance._OnOpen(); };
      this._socket.onmessage = function(data) { ACInstance._OnMessage(data); };
      this._socket.onclose = function() { ACInstance._OnClose(); };
      this.SocketState = WebSocketClient.prototype.SocketStates.Connecting;

      if (this._options.DebugMode) {
        console.log('Server started, connecting to ' + server);
      }
    },

    Send: function(command,data) {
      if (typeof data === 'object') {
        data = JSON.stringify(data);
      }

      this._socket.send(command+" "+data);

      if (this._options.DebugMode) {
        console.log('Sent data to server: ' + command+" "+data);
      }
    },

    Stop: function() {
      this._socket.close();

      if (this._options.DebugMode) {
        console.log('Connection stopped by client');
      }
    },

    Connected: function() { },
    Disconnected: function() { },
    MessageReceived: function() { },

    _OnOpen: function() {
      var instance = this;
      this.SocketState = WebSocketClient.prototype.SocketStates.Open;

      if (this._options.DebugMode) {
        console.log('Connected.');
      }

      this.Connected();
    },

    _OnMessage: function(event) {
      var instance = this;

      this._lastReceive = (new Date()).getTime();

      if (this._options.DebugMode) {
        console.log('Message received: ' + JSON.stringify(event.data));
      }

      this.MessageReceived(event);
    },

    _OnClose: function() {
      var instance = this;
      if (this._options.DebugMode) {
        console.log('Connection closed.');
      }

      this.SocketState = WebSocketClient.prototype.SocketStates.Closed;

      this.Disconnected();
    }
  };

  WebSocketClient.prototype._defaultOptions = {
    Port: 81,
    Server: '',
    Action: '',

    Connected: function() { },
    Disconnected: function() { },
    MessageReceived: function(data) { },

    DebugMode: false
  };

  function MergeDefaults(o1, o2) {
    var o3 = {};
    var p = {};

    for (p in o1) {
      o3[p] = o1[p];
    }

    for (p in o2) {
      o3[p] = o2[p];
    }

    return o3;
  }

  window.WebSocketClient = WebSocketClient;
  window.MergeDefaults = MergeDefaults;

  if(window.MozWebSocket){
    window.WebSocket = MozWebSocket;
  }
})(window);

