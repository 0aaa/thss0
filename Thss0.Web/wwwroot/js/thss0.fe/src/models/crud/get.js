import React from "react"

class Get extends React.Component {
    constructor(props) {
        super(props)
        this.url = decodeURIComponent(props.match.params.href)
        this.state = {
            content: []
        }
    }
    async componentDidMount() {
        const response = await fetch(this.url)
        const content = await response.json()
        this.setState({ content })
    }
    render() {
        return (
            <>
                <h5>{Object.values(this.state.content)[0]}</h5>
                <dl>
                    {Object.keys(this.state.content).map(cntnt =>
                        <>
                            <dt>{cntnt}</dt>
                            <dd>{this.state.content[cntnt]}</dd>
                        </>
                    )}
                </dl>
                <button onClick={this.props.history.goBack()} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
export default Get