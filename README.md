# Local Ollama Chatbot with ASP.NET Core API & Web UI

This project shows you how to build a simple interactive chat application. It uses a **Small Language Model (SLM)** like Phi-3, which runs right on your local machine via Ollama, and integrates it with an ASP.NET Core Web API and a clean HTML/CSS/JavaScript frontend.

---

## ✨ Features

* **Local SLM Inference:** Uses [Ollama](https://ollama.com/) to run powerful SLMs (e.g., Phi-3) directly on your Windows PC, keeping your data local and private.
* **ASP.NET Core Web API Backend:** A lightweight API built with **.NET 8.0** that acts as the bridge between your frontend and Ollama.
* **Semantic Kernel Integration:** Leverages [Microsoft Semantic Kernel](https://learn.microsoft.com/en-us/semantic-kernel/) to make interacting with the local SLM via Ollama super easy.
* **Single-File Web UI:** A **responsive, full-browser-space** chat interface built with plain HTML, CSS, and JavaScript. It's served directly by the ASP.NET Core application for simplicity.
* **Conversation History:** The UI and API work together to maintain context for **multi-turn conversations**, so the SLM remembers what you've discussed.

---

## 🚀 Technologies Used

* **Backend:**
    * ASP.NET Core 8.0
    * C#
    * Microsoft Semantic Kernel (v1.x, including `Microsoft.SemanticKernel.Connectors.Ollama` pre-release)
* **SLM Runtime:**
    * [Ollama](https://ollama.com/)
    * [Phi-3 Mini](https://ollama.com/library/phi3) (or other compatible models from the Ollama library)
* **Frontend:**
    * HTML5
    * CSS3
    * JavaScript (ES6+)

---

## 📋 Prerequisites

Before you start, make sure you have the following installed on your **Windows PC**:

1.  **Ollama:**
    * Download and install Ollama from [https://ollama.com/download/windows](https://ollama.com/download/windows).
    * **System Requirements for Ollama:** Your PC should meet the minimum requirements (Windows 10 22H2+ / 11, **8GB+ RAM** for running models, 10GB+ storage. A **GPU is highly recommended** for faster inference).
2.  **.NET 8.0 SDK:**
    * Download and install the .NET 8.0 SDK from [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0).
3.  **Visual Studio 2022 (Community Edition is fine):**
    * Download and install Visual Studio 2022 from [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/).
    * Make sure you select the "**ASP.NET and web development**" workload during installation.

---

## ⚙️ Setup and Running Locally

Follow these steps precisely to get the project up and running on your local machine:

### 1. Set up Ollama

This step is **critical** as it provides the local language model that your application will communicate with.

* **Start Ollama:** After installation, Ollama usually starts automatically. You should see its icon in your system tray (bottom-right corner of your taskbar). If you don't, launch `Ollama` from your Start Menu.
* **Pull the Model:** Open **PowerShell** or **Command Prompt** and download the Phi-3 Mini model. This model offers a good balance of performance and size for local machines.
    ```bash
    ollama pull phi3:latest
    ```
    This download might take a few minutes depending on your internet speed and the model's size (Phi-3 Mini is a few gigabytes).

### 2. Get the ASP.NET Core Project

* **Clone the Repository:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/OllamaWebApp.git](https://github.com/YOUR_USERNAME/OllamaWebApp.git)
    cd OllamaWebApp
    ```
    *(Remember to replace `YOUR_USERNAME` with your actual GitHub username if you've forked or cloned your own repository)*
* **Open the Project in Visual Studio:**
    * Open Visual Studio 2022.
    * Go to `File` > `Open` > `Project/Solution...`
    * Navigate to the `OllamaWebApp` folder (the one containing `OllamaWebApp.sln` and the `OllamaWebApp` project folder) and select the `OllamaWebApp.sln` file.
* **Restore NuGet Packages:**
    * Once the project loads, in the **Solution Explorer** (usually on the right), right-click on the `OllamaWebApp` project.
    * Select `Manage NuGet Packages...`
    * Go to the `Browse` tab.
    * **Crucially:** At the top of the NuGet Package Manager window, make sure the **"Include prerelease"** checkbox is ticked, or the version dropdown is set to **"Latest prerelease"**.
    * Verify that `Microsoft.SemanticKernel` and `Microsoft.SemanticKernel.Connectors.Ollama` are installed. If not, search for them and install the latest **prerelease** versions.
* **Build the Project:**
    * From the Visual Studio menu, select `Build` > `Build Solution`. Make sure there are no build errors.

### 3. Run the Application

1.  **Start the ASP.NET Core API:**
    * In Visual Studio, press `F5` or click the green "Start" button.
    * This will launch the ASP.NET Core API. A console window will appear showing server logs, and your default web browser will open.
    * The browser will automatically open to the root URL of your application (e.g., `https://localhost:7093/`). This URL now directly serves the frontend UI.
    * **Keep this API running** throughout your interaction with the chat UI.
2.  **Interact with the Chatbot UI:**
    * Once the browser opens to your API's root URL, you'll see the chat interface.
    * Type your messages in the text area and press `Enter` or click the `Send` button.
    * The application will send your message (along with previous conversation history for context) to your local Ollama instance via the ASP.NET Core API, and display the SLM's response.

---

## 💡 Important Notes

* **Conversation History:** The UI maintains conversation history only within the browser's current tab. If you **close or refresh the browser tab**, the chat history for that session will be reset. The backend API does not persist chat sessions across requests in this simple example.
* **API URL:** The `API_URL` in `wwwroot/index.html` is set to a relative path (`/ollama/chat`). This works perfectly because the frontend is now served directly by the ASP.NET Core API from the same origin.
* **Ollama Resource Usage:** Running SLMs locally can consume significant RAM and GPU resources. Monitor your system performance if you notice slowdowns.

---

## 🤝 Troubleshooting

* **"Error: Connection refused" or "Internal Server Error" in browser/console:**
    * **Most Common Cause:** Ollama is **not running**, or the `phi3` model is **not pulled**.
    * **Solution:** Ensure Ollama is running in your system tray. Open PowerShell and run `ollama list` to verify `phi3` is downloaded. If not, run `ollama pull phi3:latest`. **Restart your ASP.NET Core API** in Visual Studio after fixing this.
* **`AddOllamaChatCompletion` not found / NuGet Package Errors:**
    * **Cause:** The `Microsoft.SemanticKernel.Connectors.Ollama` package is not correctly installed, or prerelease packages are not enabled in NuGet.
    * **Solution:** In Visual Studio's NuGet Package Manager, ensure "Include prerelease" is checked/selected and install `Microsoft.SemanticKernel.Connectors.Ollama`. **Rebuild the project** (`Build > Rebuild Solution`).
* **Chat History not working / Bad Request (400) from second message onwards:**
    * **Cause:** The backend API's `OllamaController.cs` is likely not correctly deserializing the `History` array from the JavaScript frontend (due to an enum mismatch).
    * **Solution:** Verify that your `OllamaController.cs` includes the `SimpleChatMessage` DTO and the manual `AuthorRole` parsing logic within the `Chat` method as detailed in the project's C# code. **Rebuild and restart your API.**
* **Slow Responses from SLM:**
    * **Cause:** Running LLMs on CPU without a compatible GPU, or using a very large model.
    * **Solution:** Ensure your GPU drivers are up-to-date if you have an NVIDIA or AMD GPU. Consider pulling a smaller quantized version of a model (e.g., `phi3:mini` or `phi3:q4_0` if available) with `ollama pull [model_name]:[tag]`.
* **UI not displaying / Page not found at root URL:**
    * **Cause:** Static file middleware not configured correctly, or `index.html` not in `wwwroot`.
    * **Solution:** Ensure `index.html` (and `style.css`, `script.js`) are directly inside the `wwwroot` folder of your `OllamaWebApp` project. Verify `app.UseDefaultFiles()` and `app.UseStaticFiles()` are called in `Program.cs` **before** `app.MapControllers()`.

---