
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
                <div className="text-muted">{timestampFromUrl(url)}</div>
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
                <div className="text-muted">{timestampFromUrl(latestUrl)}</div>
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

    const widthFromUrl = (url) => {
        return basename(url).split(" ")[2].split('x')[0];
    }

    const heightFromUrl = (url) => {
        return basename(url).split(" ")[2].split('x')[1];
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
                    <div className="text-nowrap overflow-scroll m-2 bg-light border shadow" >
                        {
                            Object.keys(grabber.urls)
                                .map(x => grabber.urls[x])
                                .map(url => (
                                    <a href={url} key={url}>
                                        <img src={url + "-thumb-skinny.jpg"} alt={url} width="25" height="500" />
                                    </a>
                                ))
                        }
                    </div>
                ) : ""
            }

        </div>
    );
}

export default GrabberDetails;