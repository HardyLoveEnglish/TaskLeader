(function(window, $) {
  
	$( "#tabs" ).tabs();
  $("#actions").Grille();
				
  Modernizr.load({
    test: Modernizr.websockets,
    nope: 'js/lib/web-socket-js/web_socket.js'
  });

  // Set URL of your WebSocketMain.swf here, for web-socket-js
  WEB_SOCKET_SWF_LOCATION = 'js/lib/web-socket-js/WebSocketMain.swf';
  WEB_SOCKET_DEBUG = true;

  var WSclient = {};
  var me = {};
  
  Connect();
  
  function Connect() {
    // If we're using the Flash fallback, we need Flash.
    if (!window.WebSocket && !swfobject.hasFlashPlayerVersion('10.0.0')) {
      alert('Flash Player >= 10.0.0 is required.');
      return;
    }

    // Set up the WebSocketClient object
    WSclient = new WebSocketClient({
      Server: 'localhost',
      Port: '2012',
      Action: 'TLgui',
      DebugMode: true
    });

    $('#status').removeClass('offline').addClass('pending').text('Connecting...');

    WSclient.Connected = function() {
      $('#status').removeClass('pending').addClass('online').text('Online');
      init();
    };

    WSclient.Disconnected = function() {
      $('#status').removeClass('pending').removeClass('online').addClass('offline').text('Offline');
    };

    WSclient.MessageReceived = function(event) {
      ParseResponse(event.data);
    };

    WSclient.Start();
  };
 
  function init() {
    WSclient.Send('GETLIST',{ type:0});
    WSclient.Send('GETLIST',{ type:1, db:"Perso"});
  }
 
  function LogMessage(message) {
    var p = $('<p></p>').text(message);
    $('#results').prepend(p);
  }

  function ParseResponse(response) {  
    var message = $.parseJSON(response);//TODO: il faut un try catch

    if (message.answerType == 0) { //dbs_list
      $.each(message.data,function(index,value) {
        $("#manuelDBcombo").append('<option>'+value.label+'</option>');
        $("#manuelDBcombo").show();
      });
    }else if (message.answerType == 1) { //entities_list
      $.each(message.data,function(index,value) {
        $("#manuelSelects").addMultipleSelect({
          label: value.label,
          displayAll: true,
          values: value.values
        });
      });
    }
  }
  
})(window, jQuery);
