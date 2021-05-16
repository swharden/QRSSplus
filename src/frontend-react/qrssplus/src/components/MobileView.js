import React from 'react';
import ModalImage from "react-modal-image";

function MobileView(props) {

    if (!props.grabberStats || Object.keys(props.grabberStats).length === 0) {
        return (<div>Loading grabbers...</div>);
    }

    const grabbers = props.grabberStats.grabbers;

    const getImage = (grabber) => {
        const latestUrl = grabber.urls[grabber.urls.length - 1];
        const basename = latestUrl.substr(latestUrl.lastIndexOf('/') + 1);
        return (
            <div className="d-inline-block m-1 border shadow-sm" key={basename}>
                <ModalImage
                    small={latestUrl + "-thumb-auto.jpg"}
                    large={latestUrl}
                    alt={basename}
                />
            </div>
        )
    }

    return (
        <div>
            <a
                className="btn btn-primary m-1 shadow-sm"
                href="./"
                role="button">
                Desktop View
            </a>
            <div>
                {
                    Object.keys(grabbers)
                        .filter(id => grabbers[id].urls.length > 0)
                        .map(id => getImage(grabbers[id]))
                }
            </div>
        </div>
    )
}

export default MobileView;