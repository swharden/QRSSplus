
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

    return (
        <table className="table table-hover border shadow">
            <thead className="bg-dark text-light">
                <tr>
                    <th>ID</th>
                    <th>Name</th>
                    <th>Callsign</th>
                    <th>Location</th>
                    <th>Image URL</th>
                    <th>Website URL</th>
                    <th>Unique Grabs</th>
                    <th>Last Update</th>
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
                                <td><a href={grabber.siteUrl} target="_blank" rel="noreferrer">site</a></td>
                                <td>{grabber.urls.length > 0 ? grabber.urls.length : "--"}</td>
                                <td>{grabber.lastUniqueAgeDays >= 1 ? grabber.lastUniqueAgeDays + " days" : ""}</td>
                            </tr>
                        ))
                }
            </tbody>
        </table>
    )
}

export default Dashboard;