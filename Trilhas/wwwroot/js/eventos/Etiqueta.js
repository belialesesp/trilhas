function gerarEtiqueta() {
	var cursistaId = document.getElementById('cursistaId').innerHTML;
	var eventoId = document.getElementById('eventoId').innerHTML;

	if (cursistaId.toString().length < 6) {
		var qntd = 6 - cursistaId.toString().length
		cursistaId = "0".repeat(qntd) + cursistaId.toString();
	}

	if (eventoId.toString().length < 6) {
		var qntd = 6 - eventoId.toString().length
		eventoId = "0".repeat(qntd) + eventoId.toString();
	}

	JsBarcode("#barcode", cursistaId +"-"+ eventoId);
};
