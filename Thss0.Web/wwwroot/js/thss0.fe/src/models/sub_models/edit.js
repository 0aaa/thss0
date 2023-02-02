class Edit extends React.Component {
    handleEdit(event) {
        event.preventDefault()
    }
    render() {
        return (
            <>
                <h5>Edit</h5>
                <form onSubmit={this.handleEdit} className="w-50">

                </form>
            </>
        )
    }
}