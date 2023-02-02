class Delete extends React.Component {
    constructor(props) {
        super(props)
        this.url = decodeURIComponent(props.match.params.href)
        this.state = {
            content: {}
        }
    }
    handleDelete(event) {
        event.preventDefault()
    }
    async componentDidMount() {
        const RESPONSE = await fetch(this.url)
        const content = await RESPONSE.json()
        this.setState({content})
    }
    render() {
        return (
            <>
                <h5>Delete {Object.values(this.state.content)[0]}</h5>
                <form onSubmit={this.handleDelete} className="w-50">
                    <table className="table">
                        <tbody>
                            {Object.keys(this.state.content)}

                            {/* to finish */}
                        </tbody>
                    </table>
                </form>
            </>
        )
    }
}