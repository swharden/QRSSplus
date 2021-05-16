function Banner(props) {

    const switchToDesktop = () => {
        window.open("./");
    }

    const mobile = () => {
        return (
            <header>
                <div className="display-1 fw-normal mt-4">QRSS Plus</div>
                <div class="form-check d-inline-block my-1">
                    <input class="form-check-input" type="checkbox" value="" id="flexCheckChecked" checked={true} onChange={switchToDesktop} />
                    <label class="form-check-label fw-lighter" for="flexCheckChecked">
                        Low-Bandwidth View
                    </label>
                </div>
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