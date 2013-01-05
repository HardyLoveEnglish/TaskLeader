(function( $ ) {

	$.fn.addMultipleSelect = function(options) {
		$("<div></div>").appendTo(this).MultipleSelect(options);
		return this; //To be chainable
	}

	$.widget( "Taskleader.MultipleSelect", {
 
		// These options will be used as defaults
		options: {
			label: "",
			displayAll: false,
			displayImage: false,
			values: []
		},
		
		// ID of "all" checkbox element
		boxID: '',		
		// ID of span element containing all the values
		spanID: '',
		
		// Set up the widget
		_create: function() {
		
			//Insert label		
			this.element.prepend(this.options.label);
			
			//Insert "All" checkbox if required.
			if (this.options.displayAll) {
				this.boxID = $('<input type="checkbox"/>').appendTo(this.element).uniqueId().attr("id");
				$(this.element).append('<label for="'+this.boxID+'">Tous</label>');
				$("#"+this.boxID).button().click($.proxy(this._allClicked, this)); //Keep "this" linked to the widget
			}
			
			//Insert values
			var span = $('<span></span>').appendTo(this.element);
			this.spanID = span.uniqueId().attr("id");
			$.each(this.options.values,function(index,value) {
				var id = $('<input type="checkbox"/>').appendTo(span).uniqueId().attr("id");
				span.append('<label for="'+id+'">'+value+'</label>');
			});
			span.appendTo(this.element).buttonset();
			$("#"+this.spanID).click($.proxy(this._itemClicked, this));
		},
		
		// Called when "all" checkbox has been selected
		_allClicked: function() {		
			$("#"+this.spanID+" > input").prop("checked",$("#"+this.boxID).prop("checked"));
			$("#"+this.spanID).buttonset("refresh");
		},
		
		// Called when an item in the list has been selected
		_itemClicked: function() {
			if ($("#"+this.boxID).prop("checked"))
				$("#"+this.boxID).prop("checked",false).button("refresh");
		},
		
		// Use the destroy method to clean up any modifications your widget has made to the DOM
		_destroy: function() {
		}
  });
  
  $.widget( "Taskleader.Grille", {
  
    // These options will be used as defaults
		options: {
      columns: [
        { "sTitle": "Engine" },
        { "sTitle": "Browser" },
        { "sTitle": "Platform" },
        { "sTitle": "Version", "sClass": "center" },
        {
          "sTitle": "Grade",
          "sClass": "center",
          "fnRender": function(obj) {
            var sReturn = obj.aData[ obj.iDataColumn ];
            if ( sReturn == "A" ) {
              sReturn = "<b>A</b>";
            }
            return sReturn;
          }
        }
      ],
      rows: [
        [ "Trident", "Internet Explorer 4.0", "Win 95+", 4, "X" ],
        [ "Trident", "Internet Explorer 5.0", "Win 95+", 5, "C" ],
        [ "Trident", "Internet Explorer 5.5", "Win 95+", 5.5, "A" ],
        [ "Trident", "Internet Explorer 6.0", "Win 98+", 6, "A" ],
        [ "Trident", "Internet Explorer 7.0", "Win XP SP2+", 7, "A" ],
        [ "Gecko", "Firefox 1.5", "Win 98+ / OSX.2+", 1.8, "A" ],
        [ "Gecko", "Firefox 2", "Win 98+ / OSX.2+", 1.8, "A" ],
        [ "Gecko", "Firefox 3", "Win 2k+ / OSX.3+", 1.9, "A" ],
        [ "Webkit", "Safari 1.2", "OSX.3", 125.5, "A" ],
        [ "Webkit", "Safari 1.3", "OSX.3", 312.8, "A" ],
        [ "Webkit", "Safari 2.0", "OSX.4+", 419.3, "A" ],
        [ "Webkit", "Safari 3.0", "OSX.4+", 522.1, "A" ]
      ],
      language: {
        "sProcessing":     "Traitement en cours...",
        "sSearch":         "Rechercher&nbsp;:",
        "sLengthMenu":     "Afficher _MENU_ &eacute;l&eacute;ments",
        "sInfo":           "Affichage de l'&eacute;lement _START_ &agrave; _END_ sur _TOTAL_ &eacute;l&eacute;ments",
        "sInfoEmpty":      "Affichage de l'&eacute;lement 0 &agrave; 0 sur 0 &eacute;l&eacute;ments",
        "sInfoFiltered":   "(filtr&eacute; de _MAX_ &eacute;l&eacute;ments au total)",
        "sInfoPostFix":    "",
        "sLoadingRecords": "Chargement en cours...",
        "sZeroRecords":    "Aucun &eacute;l&eacute;ment &agrave; afficher",
        "sEmptyTable":     "Aucune donnée disponible dans le tableau",
        "oPaginate": {
            "sFirst":      "Premier",
            "sPrevious":   "Pr&eacute;c&eacute;dent",
            "sNext":       "Suivant",
            "sLast":       "Dernier"
        },
        "oAria": {
            "sSortAscending":  ": activer pour trier la colonne par ordre croissant",
            "sSortDescending": ": activer pour trier la colonne par ordre décroissant"
        }
      }
    },
     
    // Set up the widget
		_create: function() {
      $(this.element).dataTable({
        "aaData": this.options.rows,
        "aoColumns": this.options.columns,
        "bJQueryUI": true,
        "oLanguage": this.options.language
      });	
    },
    
    // Use the destroy method to clean up any modifications your widget has made to the DOM
		_destroy: function() {
		}
  });
  
 }( jQuery ) );