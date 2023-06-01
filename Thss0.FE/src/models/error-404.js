import { useNavigate } from 'react-router-dom'

const Error404 = () => {
    const navigate = useNavigate()
    return (
        <>
            <h5>Error 404</h5>
            <button onClick={() => navigate(-1)} className="btn btn-outline-primary">Return</button>
        </>
    )
}
export default Error404