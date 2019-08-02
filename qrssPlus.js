window.onload = function() {
    generateContent();
}

function getLatestFiles(grabberID, divSource) {
    fileNames = document.getElementById(divSource).innerHTML;
    fileNames = fileNames.split("\n");
    thisGrabberFiles = [];
    fileNames.forEach(function(element) {
        element = element.trim();
        if (element.startsWith(grabberID))
            thisGrabberFiles.push(element);
    });
    thisGrabberFiles.sort();
    thisGrabberFiles.reverse();
    return thisGrabberFiles;
}

function isActive(latestGrabs) {
    hashes = [];
    latestGrabs.forEach(function(element) {
        hashes.push(element.split(".")[2]);
    });
    uniqueHashes = new Set(hashes);
    if (uniqueHashes.size > 1)
        return true;
    else
        return false;
}

function Grabber(line) {
    // given a CSV line create a gabber object
    line = line.trim();
    parts = Papa.parse(line, {
        delimiter: ","
    }).data[0];
    this.line = line;
    this.id = parts[0];
    this.callsign = parts[1];
    this.title = parts[2];
    this.name = parts[3];
    this.location = parts[4];
    this.urlSite = parts[5];
    this.urlGrab = parts[6];

    this.latestGrabs = getLatestFiles(this.id, "latestGrabs");
    this.latestStacks = getLatestFiles(this.id, "latestStacks");
    this.thumbnails = getLatestFiles(this.id, "thumbnails");

    this.latestGrab = this.latestGrabs[0];
    this.latestThumbnail = this.thumbnails[0];
    this.latestStack = this.latestStacks[0];
    this.isActive = isActive(this.latestGrabs);

    if (this.isActive)
        this.activeString = "active";
    else
        this.activeString = "inactive";

}

Grabber.prototype.toString = function() {
    return `Grabber info for ${this.id}`;
}

Grabber.prototype.getHtml = function(showLatest, showStack, showThumbs) {
    grabberHtml = "";
    grabberHtml += "<div>";
    urlQRZ = "https://www.qrz.com/lookup/" + this.callsign;
    grabberHtml += `<b>${this.id}</b> ${this.name} (<a href='${urlQRZ}'>${this.callsign}</a>) in <b>${this.location}</b>`;
    grabberHtml += ` (<a href='${this.urlSite}'>website</a></span>)`;
    grabberHtml += "</div>";
    return grabberHtml;
}

function getGrabbers() {
    csv = document.getElementById("grabberDataCsv").innerHTML;
    lines = csv.split("\n");
    var grabbers = [];
    for (i = 0; i < lines.length; i++) {
        line = lines[i].trim();
        if (line.startsWith("#"))
            continue;
        if (line.split(",").length < 5)
            continue;
        var grabber = new Grabber(line);
        grabbers.push(grabber);
    }
    return grabbers;
}

function generateContent() {

    showLatestGrab = document.getElementById("showLatestGrab").checked;
    showLatestStack = document.getElementById("showLatestStack").checked;
    showAllGrabs = document.getElementById("showAllGrabs").checked;
    showInactiveGrabbers = document.getElementById("showInactiveGrabbers").checked;
    showSummary = document.getElementById("showSummary").checked;

    html = "";
    grabbers = getGrabbers();

    // show thumbnails for active grabbers
    if (showSummary) {
        html += "<div class='title'>Active Grabber Summary</div>"
        for (var i = 0; i < grabbers.length; i++) {
            grabber = grabbers[i];
            if (grabber.isActive) {
                html += "<div style='float: left; margin: 5px;'>";
                html += `<a href='#${grabber.id}'><b>${grabber.callsign}</b><br></a>`;
                imgCode = `<img class='thumbnail' src='data/thumbs/${grabber.latestThumbnail}' />`;
                html += `<a href='data/${grabber.latestGrab}'>${imgCode}</a>`;
                html += "</div>";
            }
        }
        html += "<div style='clear: left;'></div>";
    }


    // show active grabbers
    html += "<div class='title'>Active Grabbers</div>"
    for (var i = 0; i < grabbers.length; i++) {
        grabber = grabbers[i];
        if (grabber.isActive) {
            html += "<div class='activeBlock' style='border: 1px solid black;'>";

            grabberInfo = grabber.getHtml();
            html += `<a name='${grabber.id}'></a><div class='activeTitle'>${grabberInfo}</div><br>`;

            if (showLatestGrab)
                html += `<a href='data/${grabber.latestGrab}'><img class='grab' src='data/${grabber.latestGrab}'></a>`

            if (showLatestStack)
                html += `<a href='data/averages/${grabber.latestStack}'><img class='grab' src='data/averages/${grabber.latestStack}'></a>`

            if (showAllGrabs) {
                html += "<div>";
                grabber.thumbnails.forEach(
                    function(element) {
                        originalFileName = element.replace(".thumb.", ".");
                        html += `<a href='data/${originalFileName}'><img class='thumbnail' src='data/thumbs/${element}' height='100'></a>`;
                    }
                );
                html += "</div>";
            }

            html += "</div>";

        }
    }

    // show inactive grabbers
    if (showInactiveGrabbers) {
        html += "<div class='title'>Inactive Grabbers</div>"
        for (var i = 0; i < grabbers.length; i++) {
            grabber = grabbers[i];
            if (!grabber.isActive) {
                html += "<div class='inactiveBlock' style='border: 1px solid black;'>"
                html += `<div class='inactiveTitle'>${grabberInfo}</div>`;
                html += `<div><a href='${grabber.urlGrab}'><img class='grab' src='${grabber.urlGrab}' height='200'></a></div>`;
                html += "</div>"
            }
        }
    }

    document.getElementById("content").innerHTML = html;
}