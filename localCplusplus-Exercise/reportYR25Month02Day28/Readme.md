# C++ Programming Assignment: IPv4 Socket Communication [Server Edition]

## **Overview**
In this assignment, you will develop a **server-side IPv4 socket communication program** using **C++ with Visual Studio**. Your task is to create a program that can receive data over a network and save it to a file while handling errors efficiently.

## **Objectives**
- Learn **socket programming** in C++.
- Implement **input validation** and **error handling**.
- Develop a **GUI-based application** with status updates.
- Use **TeraTerm** for testing communication.

---

## **Program Specifications**
### **1. Start the Receiving Process**
- The user clicks the **Start** button.
- The program checks the input fields for errors.
  - **If an input error is detected:**
    - Display an **error message**.
    - Set the focus to the edit box that encountered the error.
  - **If input is valid:** Proceed to the next step.

### **2. Establish a Connection**
- The program attempts to start receiving data.
- The **Start button is disabled** to prevent multiple attempts.
- **If a connection error occurs:**
  - Display an **error message** and terminate the receive process.
  - Enable the **Start** button again.
- **If the connection is successful:**
  - Update the status display to **“Waiting to receive…”**.
  - Proceed to the next step.

### **3. Accept and Receive Data**
- Accept an **incoming connection request**.
- Open a **file** to save the received data.
- Begin **receiving data** in chunks of **1024 bytes per read**.
- Write the received data into the file.
- **If a receive error occurs:**
  - Display an **error message**.
- **If data is received successfully:**
  - Display a **successful transmission message**.

### **4. Finalizing the Transmission**
- **After the transmission is complete:**
  - Close the **socket** to terminate communication.
  - Save and close the **file**.
  - Close the **port**.
  - Enable the **Start** button again.
  - Update the status display to **“Suspended”**.

---

## **Testing and Verification**
To test your application, use **TeraTerm**, a free terminal emulator.

### **Steps to Test Communication:**
1. Start the receiving process with your created application.
2. Open **TeraTerm** and connect to the specified **IP address and port**.
3. Send data using **TeraTerm**.
4. The **TeraTerm connection will be terminated** upon completion.

### **Verification Checklist:**
✔ Does the program behave according to the specifications?  
✔ Did the communication complete without errors?  
✔ Does the saved file correctly contain the received data?  

---

## **References & Resources**
- **Socket Programming Basics:** [Beginner Engineer Study](https://beginner-engineer-study.com/socket/)
- **TeraTerm Download:** [GitHub Releases](https://github.com/TeraTermProject/teraterm/releases)  
  - Recommended version: `teraterm-5.3.exe`

---

## **Additional Notes**
- Please refer to the attached **GUI reference image** to guide your implementation.
- Ensure your code is **well-documented** with meaningful comments.
- Push your completed project to your GitHub Classroom repository.

If you have any questions, feel free to reach out. Happy coding! 🚀

