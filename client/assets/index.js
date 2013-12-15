function displayData(filtres){
	$("#etiquettes")
		.append(
			'<span class="label label-primary">'+
				'<span>Filtre: manuel</span>'+
				'<a class="upload" href=""><span class="glyphicon glyphicon-info-sign"></span></a>'+
				'<a class="upload" href=""><span class="glyphicon glyphicon-remove"></span></a>'+
			'</span>'
		);
	
	// Construction de la table
	var oTable = $('#tableau').dataTable( {
		"oLanguage": {"sUrl": "assets/datatables.french.lang"},
		"iDisplayLength": 25,
		"aoColumns": [
			{ "sTitle": "id" },
			{ "sTitle": "Liens" },
			{ "sTitle": "Contexte" },
			{ "sTitle": "Sujet" },
			{ "sTitle": "Contenu", "sWidth": "20%",
			  "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
				nTd.innerHTML = sData.replace(/\r\n|\r|\n/g, '<br />');
			  }
			},
			{ "sTitle": "Deadline" },
			{ "sTitle": "Destinataire" },
			{ "sTitle": "Statut" },
			{ "sTitle": "DB" },
			{ "sTitle": "Ref" }
		],
		"aaSorting": [[ 1, "desc" ]], // Tri sur la colonne date
		"bProcessing": true, // Affichage d'une pop-up lors du chargement des données
		"bServerSide": true,
		"sAjaxSource": "../getActions",
		"fnServerData": function ( sSource, aoData, fnCallback, oSettings ) {
			oSettings.jqXHR = $.ajax({
				dataType: 'json',
				type: "POST",
				url: sSource,
				data: JSON.stringify({DTparams: aoData, filtres: filtres}),
				processData: false,
				contentType: "application/json; charset=UTF-8",
				success: fnCallback
			});
		},
		"fnInitComplete": function(oSettings, json) { // Mise en form Bootstrap de certains composants
			$('div.dataTables_filter input')
				.attr('placeholder', 'Rechercher')
				.addClass('form-control input-sm')
				.css('width', '250px');
			$('select[name=transactions_length]')
				.addClass('form-control input-sm')
				.css('width', '75px');
			$('div.dataTables_info').css('margin-bottom', '30px');
		}
	} );	
}

$(document).ready(function() {
	
	//Ajout des bases actives
	$.get('../getActiveDatabases', function(data) {
		$(data).each(function(i,dbName){
			$('<label class="btn btn-primary"></label>')
				.append('<input type="radio" name="options">'+dbName)
				.appendTo($('#manualDB'));
		});
	},"json");
		
	// Création des CritereSelect
	$.get('../getDBListentities?db=New', function(data) {
		$(data).each(function(i,entity){
			if(entity.parentID==0) // Pas d'entité parente
				$("div#manuel").addCritereSelect({
					dbName: "New",
					entityID: entity.id,
					entityName: entity.nom
				});
		});
	},"json");	

	$("#recherche").click(function(){
		displayData([{ recherche:"test", dbName:"New" }]);
	});
	
	$("#stored").click(function(){
		displayData([{ id:1, dbName:"New" }]);
	});
	
	$("#launch").click(function(){
		var criteria = new Array();
		$($("div#manuel").data( "critereSelects" )).each(function(i,critereSelect){
			var criterium = critereSelect.getListValue();
			if(criterium)
				criteria.push(criterium);
		});
		displayData([{ criteria:criteria, dbName:"New" }]);
	});	
	
});