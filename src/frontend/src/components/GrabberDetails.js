
import React from 'react';
import ModalImage from "react-modal-image";

const GrabberDetails = (props) => {

    const grabber = props.grabber;
    const showStitch = props.showStitch;
    const maxThumbnailCount = props.maxThumbnailCount;

    const latestUrl = grabber.urls[grabber.urls.length - 1];

    const renderActivityIcon = (ageMinutes) => {
        if (ageMinutes < 35)
            return (<span className="badge bg-success">Active</span>)
        if (ageMinutes < (60 * 24))
            return (<span className="badge bg-warning">Inactive ({ageMinutes} minutes)</span>)
        return (<span className="badge bg-danger">Offline ({ageMinutes / 60 / 24} days)</span>)
    };

    const renderDatedThumbnail = (url) => {
        return (
            <div className="d-inline-block m-2" key={basename(url)}>
                <div className="text-muted">{timestampFromUrl(url)} ({timestampAgeFromUrl(url)})</div>
                <div className="border shadow" style={{ width: "150px", height: "100px" }}>
                    <ModalImage
                        small={url + "-thumb-auto.jpg"}
                        large={url}
                        alt={basename(url) + "-thumb-auto"}
                    />
                </div>
            </div >
        )
    }

    const renderPrimaryImage = (latestUrl) => {
        return (
            <div className="m-2">
                <div className="text-muted">{timestampFromUrl(latestUrl)} ({timestampAgeFromUrl(latestUrl)})</div>
                <div className="mt-1 mb-1">
                    <div className="border border-dark shadow figure-img d-inline-block">
                        <ModalImage
                            small={latestUrl}
                            large={latestUrl}
                            alt={basename(latestUrl) + "-primary"}
                        />
                    </div>
                </div>
            </div>
        );
    }

    const basename = (url) => {
        return url.substr(url.lastIndexOf('/') + 1);
    }

    const timestampFromUrl = (url) => {
        const parts = basename(url).split(" ")[1].split('.');
        const timestamp = parts[3] + ":" + parts[4];
        return timestamp;
    }

    const timestampAgeFromUrl = (url) => {
        const parts = basename(url).split(" ")[1].split('.');
        const urlDate = new Date(parts[0], parts[1] - 1, parts[2], parts[3], parts[4], parts[5]);
        const urlLinuxTime = urlDate.getTime() / 1000 - urlDate.getTimezoneOffset() * 60;
        const nowDate = new Date();
        const nowLinuxTime = nowDate.getTime() / 1000;
        const urlAgeMin = (nowLinuxTime - urlLinuxTime) / 60;
        return `${Math.round(urlAgeMin)} min`;
    }

    return (
        <div id={grabber.id} key={latestUrl}>
            <h2 className="my-5 mb-0 display-4 my-0 fw-normal">{grabber.id}</h2>

            <div className="fs-5 my-0">
                {grabber.name} (<a href={"https://www.qrz.com/lookup/" + grabber.callsign}>{grabber.callsign}</a>)
                in {grabber.location} (<a href={grabber.siteUrl}>website</a>)
                &nbsp;
                {renderActivityIcon(grabber.lastUniqueAgeMinutes)}
            </div>

            {
                renderPrimaryImage(latestUrl)
            }

            {
                Object.keys(grabber.urls)
                    .map(x => grabber.urls[x])
                    .reverse()
                    .slice(0, maxThumbnailCount)
                    .map(x => renderDatedThumbnail(x))
            }

            {
                showStitch ? (
                    <div className="text-nowrap overflow-scroll m-2" style={{ height: "520px" }} >
                        {
                            Object.keys(grabber.urls)
                                .map(x => grabber.urls[x])
                                .map(url => (
                                    <div className="d-inline-block" style={{ width: "25px", height: "500px" }}>
                                        <ModalImage
                                            small={url + "-thumb-skinny.jpg"}
                                            large={url}
                                            alt={grabber.id}
                                        />
                                    </div>
                                ))
                        }
                    </div>
                ) : ""
            }

        </div>
    );
}

export default GrabberDetails;