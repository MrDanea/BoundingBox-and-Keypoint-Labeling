// State variables
var imageState = {
    width: 0,
    height: 0,
    selectedImage: "",
    stage: null,
    layer: null
};

var labelingState = {
    isHumanLabeling: false,
    currentShapes: [],
    shapesData: new Map() // Store shapes data for each image
};

$(document).ready(function () {
    // Initialize Konva stage
    labelingState.stage = new Konva.Stage({
        container: 'image-container',
        willReadFrequently: true
    });

    labelingState.layer = new Konva.Layer({ willReadFrequently: true });
    labelingState.stage.add(labelingState.layer);

    initializeEventHandlers();
});

function initializeEventHandlers() {
    // 1. Image Selection Handler
    $('#image-list').on('click', 'li', handleImageSelection);

    // 2. Labeling Type Selection Handler
    $('#labeling-ul').on('click', 'li', handleLabelingTypeSelection);

    // 3. History List Selection Handler
    $('#history-list').on('click', 'li', handleHistorySelection);

    // 4. Delete Handler
    $('#del-dane-test').click(deleteSelectedShape);

    // 5. Save Handler
    $('#commit-and-update').click(saveLabeling);
}

// 1. Image Selection Flow
function handleImageSelection() {
    resetAllStates();

    // Update selected image
    $('#image-list li').removeClass('bg-success text-white');
    $(this).addClass('bg-success text-white');

    imageState.selectedImage = $(this).text();
    const username = $('#username').text();

    loadImage(username, imageState.selectedImage);
}

function resetAllStates() {
    // Reset labeling type selection
    $('#labeling-ul li').css({
        'background-color': '',
        'color': ''
    });

    // Reset labeling state
    labelingState.isHumanLabeling = false;
    labelingState.currentShapes = [];

    // Clear history
    $('#history-list').empty();

    // Clear canvas
    if (labelingState.layer) {
        labelingState.layer.destroyChildren();
        labelingState.layer.draw();
    }
}

function loadImage(username, imageName) {
    const data = {
        userName: username,
        imageName: imageName
    };

    $.ajax({
        url: `/api/labeling/getimages`,
        method: 'POST',
        contentType: 'application/json',
        xhrFields: { responseType: 'blob' },
        data: JSON.stringify(data),
        success: handleImageLoad,
        error: function (error) {
            console.error('Image load error:', error);
            alert("Không thể tải ảnh.");
        }
    });
}

function handleImageLoad(data) {
    const objectUrl = URL.createObjectURL(data);
    const imageObj = new Image();

    imageObj.onload = function () {
        // Update stage dimensions
        imageState.width = imageObj.width;
        imageState.height = imageObj.height;
        labelingState.stage.width(imageState.width);
        labelingState.stage.height(imageState.height);

        // Draw image
        const konvaImage = new Konva.Image({
            x: 0,
            y: 0,
            image: imageObj,
            width: imageState.width,
            height: imageState.height
        });

        labelingState.layer.add(konvaImage);
        labelingState.layer.draw();

        // Load existing shapes if any
        loadExistingShapes();
    };

    imageObj.src = objectUrl;
}

// 2. Labeling Type Selection Flow
function handleLabelingTypeSelection() {
    // Reset previous selection
    $('#labeling-ul li').css({
        'background-color': '',
        'color': ''
    });

    // Update current selection
    $(this).css({
        'background-color': '#4CAF50',
        'color': 'white'
    });

    // Set labeling type
    labelingState.isHumanLabeling = ($(this).text() === "Human");

    // Initialize appropriate drawing handlers
    initializeDrawing();
}

function initializeDrawing() {
    // Remove existing event listeners
    labelingState.stage.off('mousedown mousemove mouseup click');

    if (labelingState.isHumanLabeling) {
        initializeRectangleDrawing();
    } else {
        initializeKeypointDrawing();
    }
}

function initializeRectangleDrawing() {
    let isDrawing = false;
    let startPos = null;
    let currentRect = null;

    labelingState.stage.on('mousedown', function (e) {
        isDrawing = true;
        startPos = labelingState.stage.getPointerPosition();
        currentRect = createRectangle(startPos);
    });

    labelingState.stage.on('mousemove', function () {
        if (!isDrawing) return;
        updateRectangle(currentRect, startPos);
    });

    labelingState.stage.on('mouseup', function () {
        if (!isDrawing) return;
        isDrawing = false;
        finishRectangle(currentRect);
    });
}

function initializeKeypointDrawing() {
    labelingState.stage.on('click', function () {
        const pos = labelingState.stage.getPointerPosition();
        createKeypoint(pos);
    });
}

// 3. Shape Management
function createRectangle(startPos) {
    const rect = new Konva.Rect({
        x: startPos.x,
        y: startPos.y,
        width: 0,
        height: 0,
        stroke: 'blue',
        strokeWidth: 2
    });

    labelingState.layer.add(rect);
    return rect;
}

function createKeypoint(pos) {
    const keypoint = new Konva.Circle({
        x: pos.x,
        y: pos.y,
        radius: 2,
        fill: 'red',
        stroke: 'black',
        strokeWidth: 1
    });

    labelingState.layer.add(keypoint);
    labelingState.layer.draw();

    addToHistory(keypoint, 'Keypoint');
    saveShapeData(keypoint, 'keypoint');
}

function updateRectangle(rect, startPos) {
    const pos = labelingState.stage.getPointerPosition();
    const width = pos.x - startPos.x;
    const height = pos.y - startPos.y;

    rect.width(Math.abs(width));
    rect.height(Math.abs(height));
    rect.x(width < 0 ? pos.x : startPos.x);
    rect.y(height < 0 ? pos.y : startPos.y);

    labelingState.layer.batchDraw();
}

function finishRectangle(rect) {
    if (rect.width() < 5 || rect.height() < 5) {
        rect.destroy();
        labelingState.layer.draw();
        return;
    }

    addToHistory(rect, 'Rectangle');
    saveShapeData(rect, 'rectangle');
}

function addToHistory(shape, type) {
    const index = labelingState.currentShapes.length + 1;
    const li = $('<li class="custom-li">').text(`${type} ${index}`);
    li.attr('data-id', labelingState.currentShapes.length);

    $('#history-list').append(li);
    labelingState.currentShapes.push({ shape: shape, li: li });
}

// 4. Data Management
function saveShapeData(shape, type) {
    const shapeData = {
        type: type,
        imageName: imageState.selectedImage,
        imageWidth: imageState.width,
        imageHeight: imageState.height
    };

    if (type === 'rectangle') {
        Object.assign(shapeData, {
            width: shape.width(),
            height: shape.height(),
            x: shape.x(),
            y: shape.y(),
            centerX: shape.x() + shape.width() / 2,
            centerY: shape.y() + shape.height() / 2
        });
    } else {
        Object.assign(shapeData, {
            x: shape.x(),
            y: shape.y(),
            visible: getVisible()
        });
    }

    if (!labelingState.shapesData.has(imageState.selectedImage)) {
        labelingState.shapesData.set(imageState.selectedImage, []);
    }

    labelingState.shapesData.get(imageState.selectedImage).push(shapeData);
}

function getVisible() {
    return $('#customCheck1').is(':checked') ? 1 : 0;
}
function loadExistingShapes() {
    const shapes = labelingState.shapesData.get(imageState.selectedImage) || [];
    shapes.forEach((data, index) => {
        if (data.type === 'rectangle') {
            const rect = new Konva.Rect({
                x: data.x,
                y: data.y,
                width: data.width,
                height: data.height,
                stroke: 'blue',
                strokeWidth: 2
            });
            labelingState.layer.add(rect);
            addToHistory(rect, 'Rectangle');
        } else {
            const keypoint = new Konva.Circle({
                x: data.x,
                y: data.y,
                radius: 2,
                fill: 'red',
                stroke: 'black',
                strokeWidth: 1
            });
            labelingState.layer.add(keypoint);
            addToHistory(keypoint, 'Keypoint');
        }
    });
    labelingState.layer.draw();
}

// 5. Shape Deletion
function handleHistorySelection() {
    const previousSelected = $('#history-list li.selected');
    if (previousSelected.length) {
        previousSelected.removeClass('selected');
    }
    $(this).addClass('selected');
}

function deleteSelectedShape() {
    const selected = $('#history-list li.selected');
    if (!selected.length) return;

    const index = selected.attr('data-id');
    const shapeData = labelingState.currentShapes[index];

    if (shapeData) {
        shapeData.shape.destroy();
        selected.remove();
        labelingState.currentShapes[index] = null;

        // Update indices of remaining shapes
        $('#history-list li').each(function (idx) {
            const type = labelingState.isHumanLabeling ? 'Rectangle' : 'Keypoint';
            $(this).text(`${type} ${idx + 1}`);
        });

        labelingState.layer.draw();
    }
}

// 6. Save Labeling Data
function saveLabeling() {
    const shapes = labelingState.shapesData.get(imageState.selectedImage) || [];
    if (shapes.length === 0) {
        alert("Không có dữ liệu để lưu.");
        return;
    }

    // Tạo JSON để gửi đi
    const dataToSend = {
        imageName: imageState.selectedImage,
        imageWidth: imageState.width,
        imageHeight: imageState.height,
        string_boundingBoxes: [],
        string_keypoints: []
    };

    shapes.forEach(shape => {
        if (shape.type === 'rectangle') {
            dataToSend.string_boundingBoxes.push({
                width: shape.width,
                height: shape.height,
                centerX: shape.centerX,
                centerY: shape.centerY
            });
        } else if (shape.type === 'keypoint') {
            if (shape.visible == 0) {
                dataToSend.string_keypoints.push({
                    x: 0,
                    y: 0,
                    visible: 0
                });
            } else {
                dataToSend.string_keypoints.push({
                    x: shape.x,
                    y: shape.y,
                    visible: shape.visible
                });
            }
        }
    });

    $.ajax({
        url: 'api/labeling/commitnupdate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(dataToSend), // Gửi toàn bộ thông tin
        success: function (response) {
            console.log('Save successful:', response);
            alert(response.Msg);
        },
        error: function (error) {
            console.error("Save failed:", error);
            alert("Lưu thất bại: " + error);
        }
    });
}
function downloadFile(fileName, username) {
    $.ajax({
        url: `/api/labeling/downloadlabelfile?fileName=${encodeURIComponent(fileName)}&username=${encodeURIComponent(username)}`,
        type: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            var url = window.URL.createObjectURL(data);
            var a = document.createElement('a');
            a.href = url;
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        },
        error: function (xhr, status, error) {
            console.error("Error downloading file:", error);
            alert("download: Error");
        }
    });
}
