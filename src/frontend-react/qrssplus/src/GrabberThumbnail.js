import React from 'react';

class GrabberThumbnail extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            id: props.grabber.id,
            imageUrl: props.grabber.urls[props.grabber.urls.length - 1] + "-thumb-auto.jpg"
        }
    }

    render() {
        return (
            <div className="d-inline-block m-2">
                <div>{this.state.id}</div>
                <div>
                    <a href={"#" + this.state.id}>
                        <img src={this.state.imageUrl} alt={this.state.id} width="150" height="100" className="shadow border" />
                    </a>
                </div>
            </div>
        );
    }
}

export default GrabberThumbnail;