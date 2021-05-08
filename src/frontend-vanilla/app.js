function basename(filename) {
    return filename.substr(filename.lastIndexOf('/') + 1);
}

function GetImageDimensions(filename) {
    return basename(filename).split(" ")[2].split('x');
}

function GetImageShortTime(filename) {
    const parts = basename(filename).split(" ")[1].split('.');
    return parts[3] + ":" + parts[4];
}

function UpdateThumbnails(grabbersJson) {
    const grabbers = grabbersJson['grabbers'];
    const thumbDiv = document.createElement('div');
    for (const key in grabbers) {
        const grabber = grabbers[key];
        const urls = grabber['urls'];
        if (urls.length == 0)
            continue;
        const lastUrl = urls[urls.length - 1];

        thumbDiv.innerHTML +=
            `<div class='d-inline-block m-2'>` +
            `<div>${grabber['id']}</div>` +
            `<a href='#${grabber['id']}'>` +
            `<img src='${lastUrl}-thumb-auto.jpg' width='150' height='100'>` +
            `</a>` +
            `</div>`;
    }
    document.querySelector('.container').appendChild(thumbDiv);
}

function UpdateDetails(grabbersJson) {
    const grabbers = grabbersJson['grabbers'];
    for (const key in grabbers) {
        const grabber = grabbers[key];
        const urls = grabber['urls'];
        if (urls.length == 0)
            continue;
        const lastUrl = urls[urls.length - 1];

        var thumbHtml = "";
        urls.reverse().forEach(function (url) {
            console.log(url);
            thumbHtml += `<div class='d-inline-block m-2'>` +
                `<div>${GetImageShortTime(url)}</div>` +
                `<a href='${url}'>` +
                `<img src='${url}-thumb-auto.jpg' width='150' height='100'>` +
                `</a>` +
                `</div>`;
        });

        const x = document.createElement('div');
        const imageDims = GetImageDimensions(lastUrl);
        x.innerHTML = `<a name='${grabber['id']}'>` +
            `<h3 class='fs-1 fw-light mt-5 mb-0'>${grabber['id']}</h3>` +
            `<div><a href='${lastUrl}'>` +
            `<img class='img-fluid' src='${lastUrl}' width='${imageDims[0]}' height='${imageDims[1]}'>` +
            `</a></div>` +
            `<div>${thumbHtml}</div>`;

        document.querySelector('.container').appendChild(x);
    }
}

function Update() {
    var url = 'https://qrssplus.z20.web.core.windows.net/grabbers.json';
    fetch(url, { method: 'GET' })
        .then(function (response) { return response.json(); })
        .then(function (grabbersJson) {
            UpdateThumbnails(grabbersJson)
            UpdateDetails(grabbersJson)
        });
}

Update();