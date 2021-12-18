
import React from 'react';

const Dashboard = (props) => {

    if (!props.grabberStats || Object.keys(props.grabberStats).length === 0) {
        return (<div>Loading dashboard...</div>);
    }

    const grabbers = props.grabberStats.grabbers;

    const grabberRowClass = (grabber) => {
        if (grabber.urls.length > 0)
            return ""
        if (grabber.lastUniqueAgeDays >= 7)
            return "table-danger"
        else return "table-warning"
    }

    const grabberAgeMessage = (grabber) => {
        if (grabber.lastUniqueAgeDays < 1){
            return "today";
        } else if (grabber.lastUniqueAgeDays < 999) {
            return grabber.lastUniqueAgeDays + " days ago";
        } else {
            return "never";
        }
    }

    const grabCountMessage = (grabber) => 
        (grabber.urls.length > 0) 
            ? grabber.urls.length 
            : (<span className="text-muted">--</span>);

    return (
        <table className="table table-hover border shadow">
            <thead className="bg-dark text-light">
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Callsign</th>
                    <th>Location</th>
                    <th>Image</th>
                    <th>Website</th>
                    <th>Grabs</th>
                    <th>Last Upload</th>
                </tr>
            </thead>
            <tbody>
                {
                    Object.keys(grabbers)
                        .map(id => (grabbers[id]))
                        .map(grabber => (
                            <tr key={grabber.id} className={grabberRowClass(grabber)}>
                                <td><code>{grabber.id}</code></td>
                                <td>{grabber.name}</td>
                                <td>{grabber.callsign}</td>
                                <td>{grabber.location}</td>
                                <td><a href={grabber.imageUrl} target="_blank" rel="noreferrer">image</a></td>
                                <td><a href={grabber.siteUrl} target="_blank" rel="noreferrer">website</a></td>
                                <td>{grabCountMessage(grabber)}</td>
                                <td>{grabberAgeMessage(grabber)}</td>
                            </tr>
                        ))
                }
            </tbody>
        </table>
    )
}

export default Dashboard;