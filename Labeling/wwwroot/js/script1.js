// State variables
var widthImg = 0;
var heightImg = 0;
var canDrawBoundingBox = false;
let rectangles = [];
let selectedLi = null;
var imageSelected = "";
var rectanglesByImage = new Map();
var currentRectangles = [];
var boundingBoxesByImage = {};

$(document).ready(function () {
    // Khởi tạo stage với willReadFrequently
    var stage = new Konva.Stage({
        container: 'image-container',
        willReadFrequently: true
    });

    // Image list click handler
    $('#image-list').on('click', 'li', function () {
        let username = $('#username').text();

        // Reset current selection
        $('#image-list li').removeClass('bg-success text-white');
        $(this).addClass('bg-success text-white');
        imageSelected = $(this).text();

        // Load new image
        loadImage(username, stage);
    });

    // Labeling list click handler
    $('#labeling-ul').on('click', 'li', function () {
        $('#labeling-ul li').css({
            'background-color': '',
            'color': ''
        });

        $(this).css({
            'background-color': '#4CAF50',
            'color': 'white'
        });

        var object = $(this).text();
        canDrawBoundingBox = (object === "Human");

        // Khởi tạo layer mới cho drawing
        initializeDrawing(stage);
    });

    // Rectangle list click handler
    $('#rectangle-list').on('click', 'li', function () {
        if (selectedLi) {
            selectedLi.css('background-color', '');
        }
        selectedLi = $(this);
        selectedLi.css('background-color', '#4CAF50');
    });

    // Button handlers
    $('#del-dane-test').click(deleteRectangle);
    $('#commit-and-update').click(Update);
});

function loadImage(username, stage) {
    $.ajax({
        url: `/api/labeling/getimages?doc=${encodeURIComponent(username)}`,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log('Image data received:', data);

            const objectUrl = URL.createObjectURL(data);
            const imageObj = new Image();

            imageObj.onload = function () {
                console.log('Image loaded successfully');
                console.log('Dimensions:', imageObj.width, 'x', imageObj.height);

                widthImg = imageObj.width;
                heightImg = imageObj.height;

                stage.width(widthImg);
                stage.height(heightImg);

                var layer = new Konva.Layer({ willReadFrequently: true });
                layer.add(createImage(imageObj));
                stage.add(layer);

                // Load saved rectangles
                loadRectangles(stage);
            };

            imageObj.onerror = function (error) {
                console.error('Error loading image:', error);
                alert("Lỗi khi tải ảnh");
            };

            imageObj.src = objectUrl;
        },
        error: function (error) {
            console.error('AJAX Error:', error);
            alert("Không thể tải ảnh.");
        }
    });
}

function createImage(imageObj) {
    return new Konva.Image({
        x: 0,
        y: 0,
        image: imageObj,
        width: imageObj.width,
        height: imageObj.height
    });
}

function initializeDrawing(stage) {
    var drawingLayer = new Konva.Layer({ willReadFrequently: true });
    stage.add(drawingLayer);

    let isDrawing = false;
    let startPos = null;
    let rect = null;

    // Mousedown event
    stage.on('mousedown', function (e) {
        if (!canDrawBoundingBox) return;

        isDrawing = true;
        startPos = stage.getPointerPosition();

        // Create new rectangle
        rect = new Konva.Rect({
            x: startPos.x,
            y: startPos.y,
            width: 0,
            height: 0,
            stroke: 'blue',
            strokeWidth: 2
        });

        drawingLayer.add(rect);
        drawingLayer.draw();
    });

    // Mousemove event
    stage.on('mousemove', function () {
        if (!isDrawing) return;

        const pos = stage.getPointerPosition();
        const width = pos.x - startPos.x;
        const height = pos.y - startPos.y;

        rect.width(Math.abs(width));
        rect.height(Math.abs(height));
        rect.x(width < 0 ? pos.x : startPos.x);
        rect.y(height < 0 ? pos.y : startPos.y);

        drawingLayer.batchDraw();
    });

    // Mouseup event
    stage.on('mouseup', function () {
        if (!isDrawing) return;

        isDrawing = false;

        if (rect.width() < 5 || rect.height() < 5) {
            rect.destroy();
            drawingLayer.draw();
            return;
        }

        // Create bounding box info
        const boundingBoxInfo = {
            imageName: imageSelected,
            imageWidth: widthImg,
            imageHeight: heightImg,
            width: rect.width(),
            height: rect.height(),
            x: rect.x(),
            y: rect.y(),
            centerX: rect.x() + rect.width() / 2,
            centerY: rect.y() + rect.height() / 2
        };

        // Save bounding box info
        if (!boundingBoxesByImage[imageSelected]) {
            boundingBoxesByImage[imageSelected] = [];
        }
        boundingBoxesByImage[imageSelected].push(boundingBoxInfo);

        // Create list item
        const li = $('<li class="custom-li">').text(`Rectangle ${currentRectangles.length + 1}`);
        li.attr('data-id', currentRectangles.length);
        $('#rectangle-list').append(li);

        // Save rectangle
        currentRectangles.push({ rect: rect, li: li });
        rectanglesByImage.set(imageSelected, currentRectangles);
    });
}

function loadRectangles(stage) {
    // Clear existing rectangles
    clearCurrentRectangles();

    if (rectanglesByImage.has(imageSelected)) {
        var rectangles = rectanglesByImage.get(imageSelected);
        var layer = new Konva.Layer({ willReadFrequently: true });

        rectangles.forEach(({ rect, li }) => {
            if (rect && li) {
                layer.add(rect);
                $('#rectangle-list').append(li);
                currentRectangles.push({ rect, li });
            }
        });

        stage.add(layer);
    }
}

function clearCurrentRectangles() {
    currentRectangles.forEach(({ rect, li }) => {
        if (rect) rect.destroy();
        if (li) li.remove();
    });
    currentRectangles = [];
    $('#rectangle-list').empty();
}

function deleteRectangle() {
    if (selectedLi) {
        const id = selectedLi.attr('data-id');
        const rectangle = currentRectangles[id];
        if (rectangle) {
            rectangle.rect.destroy();
            selectedLi.remove();
            selectedLi = null;
            currentRectangles[id] = null;
            rectanglesByImage.set(imageSelected, currentRectangles);
        }
    }
}

function Update() {
    const boundingBoxes = boundingBoxesByImage[imageSelected];
    if (boundingBoxes && boundingBoxes.length > 0) {
        const boundingBoxInfo = boundingBoxes[0];

        // Parse float values
        const data = {
            ...boundingBoxInfo,
            width: parseFloat(boundingBoxInfo.width),
            height: parseFloat(boundingBoxInfo.height),
            x: parseFloat(boundingBoxInfo.x),
            y: parseFloat(boundingBoxInfo.y),
            centerX: parseFloat(boundingBoxInfo.centerX),
            centerY: parseFloat(boundingBoxInfo.centerY)
        };

        $.ajax({
            url: 'api/labeling/commitnupdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                console.log('Update successful:', response);
                alert(response.Message);
            },
            error: function (xhr, status, error) {
                console.error("Update failed:", error);
                alert("Cập nhật thất bại: " + error);
            }
        });
    }
}