var widthImg = 0;
var heightImg = 0;
var widthBox = 0;
var heightBox = 0;
var xbox = 0;
var ybox = 0;
var boundingBoxInfo;
var canDrawBoundingBox = false;
let rectangles = []; 
let selectedLi = null; 
var imageSelected = "";
var rectanglesByImage = new Map();
var currentRectangles = [];
var boundingBoxesByImage = {};
var imageURL = "";
var imageURLlist;
$(document).ready(function () {
    var stage = new Konva.Stage({
        container: 'image-container',
        willReadFrequently: true
    });
    $('#image-list').on('click', 'li', function () {
        let username = $('#username').text();
        getImageUrls(username, stage);
        $('#image-list li').each(function () {
            $(this).removeClass('bg-success text-white');
        });
        $(this).addClass('bg-success text-white');
        imageSelected = $(this).text();

    });
    $('#labeling-ul').on('click', 'li', function () {
        $('#labeling-ul li').each(function () {
            $(this).css('background-color', '');  // Xóa màu nền
            $(this).css('color', '');  // Xóa màu chữ
        });
        $(this).css('background-color', '#4CAF50');  // Màu nền khi click
        $(this).css('color', 'white');  // Màu chữ khi click

        var object = $(this).text();
        canDrawBoundingBox = (object === "Human");
        drawRectangle(stage);  
    });
    $('#rectangle-list').on('click', 'li', function () {
        if (selectedLi) {
            selectedLi.css('background-color', ''); // Xóa màu nền của li cũ
        }
        selectedLi = $(this);
        selectedLi.css('background-color', '#4CAF50'); // Thêm màu nền cho li đã chọn
    });
    $('#del-dane-test').click(deleteRectangle);
    $('#commit-and-update').click(Update);
});
function imageLoading(imageObj) {
    var image = new Konva.Image({
        x: 0,
        y: 0,
        image: imageObj,
        width: imageObj.width,
        height: imageObj.height
    });
    return image;
}
function drawRectangle(stage) {
    var layer = new Konva.Layer();
    let li = null;

    stage.on('mousedown', () => {
        if (!canDrawBoundingBox) return;
        var mousePos = stage.getPointerPosition();
        let rect = new Konva.Rect({
            x: mousePos.x,
            y: mousePos.y,
            width: 2,
            height: 2,
            stroke: 'blue',
            strokeWidth: 2,
        });
        layer.add(rect);

        // Tạo li tương ứng cho rectangle mới
        li = $('<li class="custom-li">Rectangle ' + (currentRectangles.length + 1) + '</li>');
        li.attr('data-id', currentRectangles.length);
        $('#rectangle-list').append(li);

        // Lưu rectangle và li vào mảng hiện tại
        currentRectangles.push({ rect: rect, li: li });

        let isDragging = false;

        stage.on('mousemove', () => {
            if (!isDragging) return;

            const pos = stage.getPointerPosition();
            const width = pos.x - mousePos.x;
            const height = pos.y - mousePos.y;

            rect.width(Math.abs(width));
            rect.height(Math.abs(height));
            rect.x(width < 0 ? pos.x : mousePos.x);
            rect.y(height < 0 ? pos.y : mousePos.y);

            layer.batchDraw();
        });

        stage.on('mouseup', () => {
            isDragging = false;

            const widthBox = rect.width();
            const heightBox = rect.height();
            const centerX = rect.x() + widthBox / 2;
            const centerY = rect.y() + heightBox / 2;

            boundingBoxInfo = {
                imageName: imageSelected,
                imageWidth: widthImg,
                imageHeight: heightImg,
                width: widthBox,
                height: heightBox,
                x: rect.x(),
                y: rect.y(),
                centerX: centerX,
                centerY: centerY
            };
            if (!boundingBoxesByImage[imageSelected]) {
                boundingBoxesByImage[imageSelected] = [];
            }
            boundingBoxesByImage[imageSelected].push(boundingBoxInfo);
            saveCurrentRectangles(); 
        });

        isDragging = true;
    });
    stage.add(layer);
}
function saveCurrentRectangles() {
    rectanglesByImage.set(imageSelected, currentRectangles);
}
function loadRectangles(stage) {
    clearCurrentRectangles(stage);

    if (rectanglesByImage.has(imageSelected)) {
        var rectangles = rectanglesByImage.get(imageSelected);
        var layer = new Konva.Layer();

        rectangles.forEach(({ rect, li }) => {
            layer.add(rect);
            $('#rectangle-list').append(li);
            currentRectangles.push({ rect, li });
        });

        stage.add(layer);
    }
}
function clearCurrentRectangles(stage) {
    currentRectangles.forEach(({ rect, li }) => {
        rect.destroy();
        li.remove(); 
    });
    currentRectangles = []; 
}
function deleteRectangle() {
    if (selectedLi) {
        const id = selectedLi.attr('data-id');
        const rectangle = currentRectangles[id];
        rectangle.rect.destroy();
        selectedLi.remove();
        selectedLi = null;
        currentRectangles[id] = null;
        saveCurrentRectangles();
    }
}
function Update() {
    const boundingBoxInfo = getBoundingBoxInfoForSelectedImage()[0];
    if (boundingBoxInfo != null) {
        boundingBoxInfo.width = parseFloat(boundingBoxInfo.width);
        boundingBoxInfo.height = parseFloat(boundingBoxInfo.height);
        boundingBoxInfo.x = parseFloat(boundingBoxInfo.x);
        boundingBoxInfo.y = parseFloat(boundingBoxInfo.y);
        boundingBoxInfo.centerX = parseFloat(boundingBoxInfo.centerX);
        boundingBoxInfo.centerY = parseFloat(boundingBoxInfo.centerY);
        $.ajax({
            url: 'api/labeling/commitnupdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(boundingBoxInfo),
            success: function (response) {
                console.log(response);
                alert(response.Message); // Hiển thị thông báo
            },
            error: function (xhr, status, error) {
                console.error("Request failed: " + error);
            }
        });
    }
}
function getBoundingBoxInfoForSelectedImage() {
    var t = boundingBoxesByImage[imageSelected] || [];
    return t;
}
function downloadFile(fileName) {
    $.ajax({
        url: `/api/labeling/downloadlabelfile?fileName=${encodeURIComponent(fileName)}`,
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
            alert("download: Success");
        },
        error: function (xhr, status, error) {
            console.error("Error downloading file:", error);
            alert("download: Error");
        }
    });
}
// Hàm AJAX để tải ảnh từ server
function getImageUrls(username, stage) {
    $.ajax({
        url: `/api/labeling/getimages?doc=${encodeURIComponent(username)}`,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            const objectUrl = URL.createObjectURL(data);
            const imageObj = new Image();
            imageObj.src = objectUrl;
            imageObj.onload = function () {
                //
                widthImg = imageObj.width;
                heightImg = imageObj.height;
                //
                stage.width(widthImg);
                stage.height(heightImg);

                var layer = new Konva.Layer({ willReadFrequently: true });
                layer.add(imageLoading(imageObj))
                stage.add(layer);
                loadRectangles(stage);
            };
        },
        error: function () {
            alert("Không thể tải ảnh.");
        }
    });
}
function showlog(msg) {
    console.log(msg);
}

