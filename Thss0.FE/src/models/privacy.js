import { useNavigate } from "react-router-dom"

function Privacy() {
    const navigate = useNavigate()
    return (
        <>
            <p>some privacy content</p>
            <button onClick={() => navigate(-1)} className="btn btn-outline-primary">Back</button>
        </>
    )
}
export default Privacy