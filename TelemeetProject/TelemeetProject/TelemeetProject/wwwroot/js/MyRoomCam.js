var width = 247.5;    // We will scale the photo width to this
var height = 150;     // This will be computed based on the input stream
var streaming = false;
var video = null;
var canvas = null;
var photo = null;
var auto;

var videoSelect = document.getElementsByName("webcam");

//onchange
$(document).ready(function () {
    startup();
});

function startup() {
    video = document.getElementById('video');
    canvas = document.getElementById('canvas');   
    videoSelect = document.getElementsByName("webcam");

    function gotStream(stream) {
        window.stream = stream; // make stream available to console
        video.srcObject = stream;       
        video.play();
        // Refresh button list in case labels have become available
        return navigator.mediaDevices.enumerateDevices();
    }
    function handleError(error) {        
        document.getElementById('canvas').style.display = 'none';        
        if (error.name === 'TrackStartError') {
            document.getElementById('canvas').style.display = 'none';            
        }
    }
    var videoSource = "";
    for (i = 0; i < videoSelect.length; i++) {
        if (videoSelect[i].checked) {
            videoSource = videoSelect[i].value;
        }
    }
    const constraints = {
        video: { deviceId: videoSource ? { exact: videoSource } : undefined }
    };
    //document.getElementById('canvas').style.display = 'inline';
    //document.getElementById('no-image').style.display = 'none';
    navigator.mediaDevices.getUserMedia(constraints).then(gotStream).catch(handleError);

    video.addEventListener('canplay', function (ev) {
        if (!streaming) {
            video.setAttribute('width', width);
            video.setAttribute('height', height);            
            streaming = true;
        }
    }, false);    
    clearphoto();
    autoRun();
}

function autoRun() {
    clearInterval(auto);
    takepicture();
    auto = setInterval(function () {
        takepicture();
    }, 10000);
}

function clearphoto() {
    //var context = canvas.getContext('2d');
    //context.fillStyle = "#AAA";
    //context.fillRect(0, 0, canvas.width, canvas.height);
}

function takepicture() {
    var context = canvas.getContext('2d');
    if (width && height) {
        canvas.width = width;
        canvas.height = height;
        context.drawImage(video, 0, 0, width, height);

        var data = canvas.toDataURL('image/jpeg', 0.8);
        $.ajax({
            url: '/Main/InsertImage',
            method: 'post',
            dataType: 'text',
            data: {
                imageData: data
            },
            cache: false,
            cache: false,
            error: function (response) {
                console.log("fail");
            },
            success: function (response) {
                console.log("sucess");
            }
        });        
    } else {
        clearphoto();
    }
}


