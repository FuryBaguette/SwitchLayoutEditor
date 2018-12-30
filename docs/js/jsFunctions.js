function JsJSONRead(ev) {
    if (!ev[0].name.endsWith('json')) {
        alert('This is not a json file');
        return;
    }
    var reader = new FileReader();
    reader.onloadend = function (evt) {
        SwitchLayoutEditor.App.UploadJSON(new Uint8Array(evt.target.result), ev[0].name);
    }
    if (typeof ev[0] != 'undefined')
        reader.readAsArrayBuffer(ev[0]);
}

function JsSZSRead(ev) {
    if (!ev[0].name.endsWith('szs')) {
        alert('This is not a szs file');
        return;
    }
    var reader = new FileReader();
    reader.onloadend = function (evt) {
        SwitchLayoutEditor.App.UploadSZS(new Uint8Array(evt.target.result), ev[0].name);
    }
    if (typeof ev[0] != 'undefined')
        reader.readAsArrayBuffer(ev[0]);
}

function checkClick(panes) {
    var paneName;
    var json = JSON.parse(panes);
    
	// Display values of clicked pane in settings panel
    jQuery('#folderView li').click(function (event) {
        paneName = jQuery(this).clone().children().remove().end().text().replace(/\s+/g, '');
        clearValues();
        displayValues(paneName, json);
    });
    
	// When a textbox is edited, put that value in the layout object
    jQuery(".textbox").bind("change paste keyup", function() {
        var name = $(this).attr("name");
        var value = $(this).val();
        
        for (var i = 0; i <= json.length - 1; i++) {
            if (json[i].PaneName == paneName) {
                if (name === "sizeX") json[i].Size.X = parseFloat(value);
                if (name === "sizeY") json[i].Size.Y = parseFloat(value);
                if (name === "scaleX") json[i].Scale.X = parseFloat(value);
                if (name === "scaleY") json[i].Scale.Y = parseFloat(value);
                if (name === "posY") json[i].Position.Y = parseFloat(value);
                if (name === "posX") json[i].Position.X = parseFloat(value);
                if (name === "posZ") json[i].Position.Z = parseFloat(value);
            }
        }
    });
    
    jQuery("#previewBtn").click(function () {
        var canvas = jQuery('#previewCanvas');
        canvas.clearCanvas();
        var color = 'blue';
        var middleX = 1280/2;
        var middleY = 720/2;

		// Draw canvas rectangle with switch screen size
        canvas.drawRect({
            strokeStyle: color,
            strokeWidth: 4,
            width: 1280,
            height: 720,
            x: 0,
            y: 0,
            fromCenter: false
        });

        for (var i = 0; i <= json.length - 1; i++) {
            if (json[i].PaneName == 'N_ScrollArea') color = 'blue';
            else if (json[i].PaneName == 'N_ScrollWindow') color = 'red';
            else if (json[i].PaneName == 'N_GameRoot') color = 'green';
            else if (json[i].PaneName == 'N_System') color = 'pink';
            else if (json[i].PaneName == 'L_ChildLock') color = 'yellow';
            //color = '#'+ ('000000' + Math.floor(Math.random()*16777215).toString(16)).slice(-6);

            if (json[i].Size != null) {
                var sizeX = parseFloat(json[i].Size.X);
                var sizeY = parseFloat(json[i].Size.Y);
            }
            if (json[i].Position != null) {
                var posX = parseFloat(json[i].Position.X);
                var posY = parseFloat(json[i].Position.Y);
                var posZ = parseFloat(json[i].Position.Z);
            }
            if (json[i].Scale != null) {
                var scaleX = parseFloat(json[i].Scale.X);
                var scaleY = parseFloat(json[i].Scale.X);
            }
            
            var parentX = middleX + getParentMiddleX(json, json[i].Parent);
            var parentY = middleY + getParentMiddleY(json, json[i].Parent);
            var parentScale = getParentScale(json, json[i].Parent);
            
			// Draw rectangles with pane values
            canvas.drawRect({
                strokeStyle: color,
                strokeWidth: 1,
                width: (sizeX * scaleX) * parentScale.X,
                height: (sizeY * scaleY) * parentScale.Y,
                x: ((parentX + posX) - ((sizeX * scaleX) / 2)),
                y: ((parentY + posY) - ((sizeY * scaleY) / 2)),
                fromCenter: true
            });

			// Draw pane name in pane rectangle
            canvas.drawText({
              fillStyle: '#9cf',
              strokeStyle: '#25a',
              strokeWidth: 1,
              x: ((parentX + posX) - ((sizeX * scaleX) / 2)), y: ((parentY + posY) - ((sizeY * scaleY) / 2)),
              fontSize: 10,
              fontFamily: 'Verdana, sans-serif',
              text: json[i].PaneName
            });

        }
    });
    return JSON.stringify(json);
}

// Gets Parent Pane scale size
function getParentScale(json, parentName) {
    for (var i = 0; i <= json.length - 1; i++) {
        if (json[i].PaneName == parentName) {
            var scaleX = parseFloat(json[i].Scale.X);
            var scaleY = parseFloat(json[i].Scale.Y);
            return ({X: scaleX, Y: scaleY});
        }
    }
    return {X: 0, Y: 0};
}

// Gets the position of the middle of a Parent Pane. For X Axis
function getParentMiddleX(json, parentName) {
    for (var i = 0; i <= json.length - 1; i++) {
        if (json[i].PaneName == parentName) {
            var x = parseFloat(json[i].Position.X);
            var size = parseFloat(json[i].Size.X);
            var scale = parseFloat(json[i].Scale.X);
            return (x +  ((size * scale) / 2));
        }
    }
    return 0;
}

// Gets the position of the middle of a Parent Pane. For Y Axis
function getParentMiddleY(json, parentName) {
    for (var i = 0; i <= json.length - 1; i++) {
        if (json[i].PaneName == parentName) {
            var y = parseFloat(json[i].Position.Y);
            var size = parseFloat(json[i].Size.Y);
            var scale = parseFloat(json[i].Scale.Y);
            return (y + ((size * scale) / 2));
        }
    }
    return 0;
}

function clearValues() {
    jQuery('#valuesView input[name="sizeX"]').val(0);
    jQuery('#valuesView input[name="sizeY"]').val(0);
    jQuery('#valuesView input[name="scaleX"]').val(0);
    jQuery('#valuesView input[name="scaleY"]').val(0);
    jQuery('#valuesView input[name="posX"]').val(0);
    jQuery('#valuesView input[name="posY"]').val(0);
    jQuery('#valuesView input[name="posZ"]').val(0);
}

function displayValues(paneName, json) {
    for (var i = 0; i <= json.length - 1; i++) {
        if (json[i].PaneName == paneName) {
            jQuery('#valuesView input[name="sizeX"]').val(json[i].Size.X);
            jQuery('#valuesView input[name="sizeY"]').val(json[i].Size.Y);
            jQuery('#valuesView input[name="posX"]').val(json[i].Position.X);
            jQuery('#valuesView input[name="posY"]').val(json[i].Position.Y);
            jQuery('#valuesView input[name="posZ"]').val(json[i].Position.Z);
            jQuery('#valuesView input[name="scaleX"]').val(json[i].Scale.X);
            jQuery('#valuesView input[name="scaleY"]').val(json[i].Scale.Y);
        }
    }
}