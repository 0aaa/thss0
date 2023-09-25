import ReactDOM from 'react-dom/client'
import appStore from './store/store'
import { Provider } from 'react-redux'
import Navigation from './components/navigation'
import { BrowserRouter } from 'react-router-dom'

ReactDOM.createRoot(document.getElementById('root'))
  .render(
    <Provider store={appStore}>
      <BrowserRouter>
        <Navigation />
      </BrowserRouter>
    </Provider>
  )