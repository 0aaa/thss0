import { AUTH_TOKEN, HOME_PATH, USERNAME } from '../../config/consts'
import { UseRedirect, UseToast } from '../../config/hooks'

const Logout = () => {
    sessionStorage.removeItem(AUTH_TOKEN)
    sessionStorage.removeItem(USERNAME)
    UseToast('Logged out')
    UseRedirect(HOME_PATH)
}
export default Logout