import ReactDOM from 'react-dom/client'
import appStore from './store/store'
import { Provider } from 'react-redux'
import Navigation from './models/navigation'
import { BrowserRouter } from 'react-router-dom'
import { toastsArr } from './config/consts'

ReactDOM.createRoot(document.getElementById('root'))
  .render(
    <Provider store={appStore}>
      <BrowserRouter>
        <Navigation />
      </BrowserRouter>
      {toastsArr}
    </Provider>
  )