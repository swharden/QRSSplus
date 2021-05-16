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
        <>
            {
                Object.keys(grabbers)
                    .filter(id => grabbers[id].urls.length > 0)
                    .map(id => getImage(grabbers[id]))
            }
        </>
    )
}

export default MobileView;