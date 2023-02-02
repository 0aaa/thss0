class Login extends React.Component {
    async getTokenAsync() {
        const USER_INPUT = {
            name: document.getElementById('name').value,
            password: document.getElementById('password').value
        }
        const FETCH_RESULT = await fetch('api/user/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(USER_INPUT)
        })
        const DATA = await FETCH_RESULT.json()
        if (FETCH_RESULT.ok) {
            sessionStorage.setItem(AUTH_TOKEN, DATA.access_token)
            return true
        } else {
            sessionStorage.removeItem(AUTH_TOKEN)
            alert(`Error: ${FETCH_RESULT.status}`)
            return false
        }
    }
    async handleLogin(event) {
        const DEFAULT_PATH = '/clients'
        event.preventDefault()
        if (await getTokenAsync()) {
            let nav = useNavigate()
            nav(DEFAULT_PATH)
        }
        location.reload()
    }
    render() {
        return (
            <>
                <h5>Login</h5>
                <form onSubmit={this.handleLogin} className="w-50">
                    <input id="name" className="form-control" placeholder="Name"/>
                    <input type="password" id="password" className="form-control mx-1" placeholder="Password"/>
                    <input type="submit" value="Submit" className="btn btn-outline-primary"/>
                </form>
            </>
        )
    }
}
export default Login