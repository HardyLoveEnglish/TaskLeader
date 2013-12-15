// CritereSelect
;(function ( $, window, document, undefined ) {

	// The actual plugin constructor
	function CritereSelect ( element, options ) {
		this.settings = $.extend({},{
			dbName: "New",
			entityID: 1,
			entityName: "Contexte"
		},options );
		this.init(element);
	}
			
	CritereSelect.prototype = {
	
		changeAll: function (selected){
			if(selected)
				this.wrapper.selectpicker('selectAll');
			else
				this.wrapper.selectpicker('deselectAll');	
		},
		
		init: function (element) {
			var that = this;
			// Création du wrapper
			var $wrapper = $('<select class="selectpicker" multiple></select>')
				.on('change', function(){
					$allBox.prop('checked',false);
				});
			$.get('../getDBentityValues?db='+this.settings.dbName+'&entityID='+this.settings.entityID, function(data) {
				$(data).each(function(i,listValue){
					$wrapper.append('<option value='+listValue.id+' selected>'+listValue.label+'</option>');
				});
				$wrapper.selectpicker({
					noneSelectedText : 'Aucun choix',
					countSelectedText: '{0} sélectionné sur {1}'
				});
			},"json");
			
			// Création de la checkbox all
			var $allBox = $('<input type="checkbox" checked>')
				.click(function(e){
					that.changeAll($allBox.prop('checked'));
					e.stopPropagation();
				});		
				
			// Création du bouton descriptif
			var $button = $('<button type="button" class="btn btn-info"></button>')
				.append(this.settings.entityName+':')
				.append($allBox).append('Tous')
				.click(function(){
					$allBox.prop('checked',!$allBox.prop('checked'));
					that.changeAll($allBox.prop('checked'));
				});
							
			$('<div class="btn-group-vertical"></div>')
				.append($button)
				.append($wrapper)
				.appendTo(element);
			
			this.wrapper = $wrapper;
			this.allBox = $allBox;
		},
		
		getListValue: function () {
			if(this.allBox[0].checked)
				return;

			var value = new Array();
			$(this.wrapper.val()).each(function(i,id){
				value.push({id:parseInt(id)});
			});
			return {
				Key:this.settings.entityID,
				Value: value
			}
	}
	};

	$.fn["addCritereSelect"] = function ( options ) {
		return this.each(function() {
			if ( !$.data( this, "critereSelects" ) ) {
				$.data( this, "critereSelects", [new CritereSelect( this, options )] );
			}
			else
				$.data( this, "critereSelects").push(new CritereSelect( this, options ));
		});
	};

})( jQuery, window, document );

// Etiquette
;(function ( $, window, document, undefined ) {

	// Create the defaults once
	var pluginName = "etiquette";

	// The actual plugin constructor
	function Etiquette ( element, options ) {
		this.element = element;
		this.settings = $.extend({},{
			filtre: "New",
			type: 1
		},options );
		this._name = pluginName;
		this.init();
	}

	Etiquette.prototype = {
		init: function () {
			var $wrapper = $('<select class="multiselect" multiple="multiple"></select>');
			$.get('../getDBentityValues?db='+this.settings.dbName+'&entityID='+this.settings.entityID, function(data) {
				$(data).each(function(i,listValue){
					$wrapper.append('<option value='+listValue.id+'>'+listValue.label+'</option>');
				});
				$wrapper.multiselect({
					includeSelectAllOption: true,
					//selectAllText: "Tous",
					//selectAllValue: "all",
					buttonClass: 'btn btn-default'
				});
			},"json");
			
			$('<div class="input-group"></div>')
				.append('<span class="input-group-addon">'+this.settings.entityName+'</span>')
				.append('<span class="input-group-addon">Tous<input type="checkbox"></span>')
				.append($wrapper)
				.appendTo(this.element);
			this.wrapper = $wrapper;
		},
		getListValue: function () {
			var value = new Array();
			$(this.wrapper.val()).each(function(i,id){
				value.push({id:parseInt(id)});
			});
			return {
				Key:this.settings.entityID,
				Value: value
			}
		}
	};

	$.fn[ pluginName ] = function ( options ) {
		return this.each(function() {
			if ( !$.data( this, "plugin_" + pluginName ) ) {
				$.data( this, "plugin_" + pluginName, new CritereSelect( this, options ) );
			}
		});
	};

})( jQuery, window, document );
