class Register extends React.Component {
    async handleRegister(event) {
        event.preventDefault()
    }
    render() {
        return (
            <>
                <h5>Register</h5>
                <form onSubmit={this.handleRegister} className="w-50">
                    <input id="name" className="form-control" placeholder="Name"/>
                    <input type="email" id="email" className="form-control mx-1" placeholder="Email"/>
                    <input id="phoneNumber" className="form-control" placeholder="Phone number"/>
                    <input type="password" id="password" className="form-control mx-1" placeholder="Password"/>
                    <input type="password" id="password-repeat" className="form-control" placeholder="Password repeat"/>
                    <input type="submit" value="Submit" className="btn btn-outline-primary mt-1"/>
                </form>
            </>
        )
    }
}
export default Register