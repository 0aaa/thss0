import { connect } from 'react-redux'
import { updateAuth } from '../../actionCreator/actionCreator'
import { AUTH_TOKEN, LOGIN_PATH } from '../../config/consts'
import { UseRedirect, UseToast } from '../../config/hook'

const Logout = (props) => {
    props.updateAuth()
    console.log('logout')
    sessionStorage.removeItem(AUTH_TOKEN)
    UseRedirect(LOGIN_PATH)
    UseToast('Logged out')
}
const mapDispatchToProps = (dispatch) => {
    return {
        updateAuth: () => dispatch(updateAuth())
    }
}
export default connect(null, mapDispatchToProps)(Logout)