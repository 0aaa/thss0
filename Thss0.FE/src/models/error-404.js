import { useNavigate } from 'react-router-dom'

const Error404 = () => {
    const navigate = useNavigate()
    return (
        <div className="d-flex justify-content-between">
            <h5>Error 404</h5>
            <button onClick={() => navigate(-1)} className="btn btn-outline-dark border-0 border-bottom rounded-0 me-2 col-2">Return</button>
        </div>
    )
}
export default Error404