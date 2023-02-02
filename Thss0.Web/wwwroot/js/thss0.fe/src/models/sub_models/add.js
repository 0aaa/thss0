class Add extends React.Component {
    handleAdd(event) {
        event.preventDefault()
    }
    render() {
        return (
            <>
                <h5>Add new</h5>
                <form onSubmit={this.handleAdd} className="w-50">

                </form>
            </>
        )
    }
}