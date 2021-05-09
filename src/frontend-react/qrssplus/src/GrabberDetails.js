import React from 'react';

class GrabberDetails extends React.Component {
    constructor(props) {
        super(props);
        const urls = props.grabber.urls.reverse();
        const latestUrl = urls[0]
        this.state = {
            grabber: props.grabber,
            urls: urls,
            latestUrl: latestUrl
        }
    }

    renderActivityIcon() {
        if (this.state.grabber.lastUniqueAgeMinutes < 35)
            return (<span className="badge bg-success">Active</span>)
        if (this.state.grabber.lastUniqueAgeMinutes < (60 * 24))
            return (<span className="badge bg-warning">Inactive ({this.state.grabber.lastUniqueAgeMinutes} minutes)</span>)
        return (<span className="badge bg-danger">Offline ({this.state.grabber.lastUniqueAgeDays} days)</span>)
    }

    renderDatedThumbnail(url) {
        const basename = url.substr(url.lastIndexOf('/') + 1);
        const parts = basename.split(" ")[1].split('.');
        const timestamp = parts[3] + ":" + parts[4];
        return (
            <div className="d-inline-block m-2" key={basename}>
                <div className="text-muted">{timestamp}</div>
                <div>
                    <a href={url}>
                        <img
                            className="border border-dark shadow"
                            alt="alt"
                            src={url + "-thumb-auto.jpg"}
                            width="150"
                            height="100" />
                    </a>
                </div>
            </div>
        )
    }

    render() {
        return (
            <div id={this.state.grabber.id}>
                <h2 className="mt-5 mb-0 display-4 my-0 fw-normal">{this.state.grabber.id}</h2>

                <div className="fs-5 my-0">
                    {this.state.grabber.name} (<a href={"https://www.qrz.com/lookup/" + this.state.grabber.callsign}>{this.state.grabber.callsign}</a>)
                    in {this.state.grabber.location} (<a href={this.state.grabber.siteUrl}>website</a>)
                    &nbsp;
                    {this.renderActivityIcon()}
                </div>

                <figure className="mt-1 mb-1">
                    <a href={this.state.latestUrl}>
                        <img
                            className="border border-dark shadow figure-img img-fluid"
                            src={this.state.latestUrl}
                            alt={this.state.grabber.id}
                            width="600"
                            height="400"
                        />
                    </a>
                </figure>

                {
                    Object.keys(this.state.urls).map(x => this.renderDatedThumbnail(this.state.urls[x]))
                }


            </div>
        );
    }
}

export default GrabberDetails;