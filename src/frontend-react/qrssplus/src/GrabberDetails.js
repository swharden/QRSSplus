import React from 'react';

class GrabberDetails extends React.Component {
    constructor(props) {
        super(props);
        const urls = props.grabber.urls;
        const latestUrl = urls[urls.length - 1];
        this.state = {
            id: props.grabber.id,
            grabber: props.grabber,
            urls: urls,
            latestUrl: latestUrl,
        }
    }

    renderActivityIcon() {
        if (this.state.grabber.lastUniqueAgeMinutes < 35)
            return (<span className="badge bg-success">Active</span>)
        if (this.state.grabber.lastUniqueAgeMinutes < (60 * 24))
            return (<span className="badge bg-warning">Inactive ({this.state.grabber.lastUniqueAgeMinutes} minutes)</span>)
        return (<span className="badge bg-danger">Offline ({this.state.grabber.lastUniqueAgeDays} days)</span>)
    }

    basename(url) {
        return url.substr(url.lastIndexOf('/') + 1);
    }

    timestampFromUrl(url) {
        const parts = this.basename(url).split(" ")[1].split('.');
        const timestamp = parts[3] + ":" + parts[4];
        return timestamp;
    }

    /*
    urlAgeMinutes(url) {
        console.log("url:" + url);
        const parts = this.basename(url).split(" ")[1].split('.');
        const dt = new Date(new Date(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]) + " UTC");
        const age = (new Date() - dt) / 1000 / 60;
        console.log("AGE:" + age);
        return age;
    }
    */

    renderDatedThumbnail(url) {
        return (
            <div className="d-inline-block m-2" key={this.basename(url)}>
                <div className="text-muted">{this.timestampFromUrl(url)}</div>
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

    showAll() {
        this.setState({ limitThumbnails: 999 })
    }

    render() {
        return (
            <div id={this.state.grabber.id}>
                <h2 className="my-5 mb-0 display-4 my-0 fw-normal">{this.state.grabber.id}</h2>

                <div className="fs-5 my-0">
                    {this.state.grabber.name} (<a href={"https://www.qrz.com/lookup/" + this.state.grabber.callsign}>{this.state.grabber.callsign}</a>)
                    in {this.state.grabber.location} (<a href={this.state.grabber.siteUrl}>website</a>)
                    &nbsp;
                    {this.renderActivityIcon()}
                </div>

                <div>
                    <div className="text-muted">{this.timestampFromUrl(this.state.latestUrl)}</div>
                    <div className="mt-1 mb-1">
                        <a href={this.state.latestUrl}>
                            <img
                                className="border border-dark shadow figure-img img-fluid"
                                src={this.state.latestUrl}
                                alt={this.state.grabber.id}
                                width="600"
                                height="400"
                            />
                        </a>
                    </div>
                </div>

                {
                    Object.keys(this.state.grabber.urls)
                        .map(x => this.state.grabber.urls[x])
                        .reverse()
                        .map(x => this.renderDatedThumbnail(x))
                }

                <div className="text-nowrap overflow-scroll m-2 bg-light border shadow" >
                    {
                        Object.keys(this.state.grabber.urls)
                            .map(x => this.state.grabber.urls[x])
                            .map(url => (
                                <a href={url} key={url}>
                                    <img src={url + "-thumb-skinny.jpg"} alt={url} width="50" height="500" />
                                </a>
                            ))
                    }
                </div>

            </div>
        );
    }
}

export default GrabberDetails;