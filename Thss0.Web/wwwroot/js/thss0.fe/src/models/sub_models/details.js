class Details extends React.Component {
    constructor(props) {
        super(props)
        this.url = decodeURIComponent(props.match.params.href)
        this.state = {
            content: {}
        }
    }
    async componentDidMount() {
        const RESPONSE = await fetch(this.url)
        const CONTENT = await RESPONSE.json()
        this.setState({ CONTENT })
    }
    render() {
        return (
            <>
                <h5>{Object.values(this.state.CONTENT)[0]}</h5>
                <table className='table'>
                    <tbody>
                        {Object.keys(this.state.CONTENT).map(cntnt =>
                            <tr>
                                <th>{cntnt}</th>
                                <td>{this.state.CONTENT[cntnt]}</td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button onClick={() => this.props.history.goBack()} className='btn btn-outline-danger'>Return</button>
            </>
        )
    }
}
export default Details