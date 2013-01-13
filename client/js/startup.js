(function(window, $) {
  
	$( "#tabs" ).tabs();

  var WSclient = {};
  
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
    WSclient.Send('GETLIST',{ type:3, filters:[{ dbName:"Perso", nom:"Test" }]});
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
    } else if (message.answerType == 1) { //entities_list
      $.each(message.data,function(index,value) {
        $("#manuelSelects").addMultipleSelect({
          label: value.label,
          displayAll: true,
          values: value.values
        });
      });
    } else if (message.answerType == 3) { //actions_list
      $("#actions").Grille({
        columns: message.data.cols,
        rows: message.data.rows
      });
    }
  }
  
})(window, jQuery);
