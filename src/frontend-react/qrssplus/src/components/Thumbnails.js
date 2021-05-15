
import React from 'react';

const Thumbnails = (props) => {

    if (!props.grabberStats || Object.keys(props.grabberStats).length === 0) {
        return (<div>Loading thumbnails...</div>);
    }

    const grabbers = props.grabberStats.grabbers;

    const getThumbnail = (grabber) => {
        return (
            <div className="d-inline-block m-2" key={grabber.id}>
                <div>{grabber.id}</div>
                <div>
                    <a href={"#" + grabber.id}>
                        <img
                            src={grabber.urls[grabber.urls.length - 1]}
                            alt={grabber.id}
                            width="150"
                            height="100"
                            className="shadow border" />
                    </a>
                </div>
            </div>
        );
    }

    const activeGrabbers = Object.keys(grabbers).filter(id => grabbers[id].urls.length > 0);

    return (
        <div className="my-5">
            <h1>Active Grabbers ({activeGrabbers.length} of {Object.keys(grabbers).length})</h1>
            {activeGrabbers.map(id => getThumbnail(grabbers[id]))}
        </div>
    );
}

export default Thumbnails;