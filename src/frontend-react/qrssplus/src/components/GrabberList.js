
import React from 'react';
import GrabberDetails from './GrabberDetails';

const GrabberList = (props) => {

    if (!props.grabberStats || Object.keys(props.grabberStats).length === 0) {
        return (<div>Loading grabbers...</div>);
    }

    const grabbers = props.grabberStats.grabbers;
    const maxGrabberCount = props.maxGrabberCount;

    return (
        <section>
            <h1>Active Grabbers</h1>
            {
                Object.keys(grabbers)
                    .filter(id => grabbers[id].urls.length > 0)
                    .slice(0, maxGrabberCount)
                    .map(id => <GrabberDetails key={id} grabber={grabbers[id]} showStitch={false} maxThumbnailCount={5} />)
            }
        </section>
    );
}

export default GrabberList;