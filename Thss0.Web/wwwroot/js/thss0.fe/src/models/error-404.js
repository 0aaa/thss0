import { useNavigate } from "react-router-dom"

const Error404 = () => {
    const navigation = useNavigate()
    return (
        <>
            <h5>Error 404</h5>
            <button onClick={() => navigation(-1)} className="btn btn-outline-primary">Return</button>
        </>
    )
}
export default Error404