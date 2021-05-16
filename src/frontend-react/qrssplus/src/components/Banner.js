function Banner(props) {

    const mobile = () => {
        return (
            <header>
                <h1 className="display-3 fw-normal mb-0">QRSS Plus <i>mobile</i></h1>
                <h3 className="fw-lighter ms-1">Low-Bandwidth View of Active Grabbers</h3>
                <div className="ms-1"><a href="./">switch to desktop mode</a></div>
            </header>
        )
    }

    const desktop = () => {
        return (
            <header>
                <h1 className="display-1 fw-normal mb-0">QRSS Plus</h1>
                <h2 className="fw-lighter mt-0">Automatically Updating Active QRSS Grabbers List</h2>
                <div className="">by <a href="https://swharden.com">Scott Harden (AJ4VD)</a> and <a href="https://sites.google.com/view/andy-g0ftd">Andy (G0FTD)</a>
                </div>
            </header>
        )
    }

    return props.mobile ? mobile() : desktop();
}

export default Banner