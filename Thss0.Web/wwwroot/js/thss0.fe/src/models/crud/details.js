import React from "react"
import { useNavigate, useParams } from "react-router-dom"
import API_URL from "../../config/consts"

class Details extends React.Component {
    constructor(props) {
        super(props)
        this.url = API_URL + props.params.entityName + '/' + props.params.id
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
                <h5>{this.state.content['name']}</h5>
                {Object.keys(this.state.content).map(cntnt =>
                    <dl key={cntnt}>
                        <dt>{cntnt}</dt>
                        <dd>{this.state.content[cntnt].length > 0 ? this.state.content[cntnt] : 'Empty'}</dd>
                    </dl>
                )}
                <button onClick={() => this.props.navigate(-1)} className="btn btn-outline-primary">Back</button>
            </>
        )
    }
}
function DetailsRouter(props) {
    return <Details {...props} params={useParams()} navigate={useNavigate()} />
}
export default DetailsRouter