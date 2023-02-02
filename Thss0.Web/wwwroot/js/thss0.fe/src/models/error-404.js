const ERROR_404 = (props) => {
    return (
        <>
            <h5>Error 404</h5>
            <button onClick={() => props.history.goBack()} className='btn btn-outline-danger'>Return</button>
        </>
    )
}
export default ERROR_404