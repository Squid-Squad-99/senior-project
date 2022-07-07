import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import Test from "Test";
import reportWebVitals from "./reportWebVitals";
import { MoralisProvider } from "react-moralis";
import { NotificationProvider } from "web3uikit";
import AppStateProvider from "context/AppContextProvider";

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
root.render(
  <MoralisProvider
    // initializeOnMount={false}
    appId={process.env.REACT_APP_MORALIS_APP_ID || ""}
    serverUrl={process.env.REACT_APP_MORALIS_SERVER_URL || ""}
  >
    <NotificationProvider>
      <AppStateProvider>
        {/* <App /> */}
        <Test />
      </AppStateProvider>
    </NotificationProvider>
  </MoralisProvider>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
