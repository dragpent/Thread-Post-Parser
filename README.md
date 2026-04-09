# 🧾 Thread Post Parser - Usage Guide

This tool extracts and processes forum/thread HTML into structured post data (Date, Name, Post) and exports it to CSV.

---

## 🚀 How to Use

### 1. View Page Source
Open the webpage you want to extract data from and:
- Right-click → **View Page Source**
- Or press `Ctrl + U`

---

### 2. Click "Copy HTML" Button
In the application:
- Click **Copy HTML**
- This will capture the page HTML for parsing

---

### 3. Paste HTML into Box
- Open the HTML input window
- Paste the copied HTML into the text box

---

### 4. Save and Add (Multiple Pages)
If you have more than one page to process:
- Click **Save and Add**
- This will store the current page’s HTML and allow you to add more pages

---

### 5. Save and Close (Finish Input)
When you are done adding all pages:
- Click **Save and Close**
- This will process all stored HTML pages into structured data

---

### 6. Review Output and Export CSV
- Review parsed results in the DataGrid
- Click **Save**
- This will generate a `.csv` file on your Desktop with a timestamp

---

## 📁 Output Format

The exported CSV will include:
Date, Name, Post


Each row represents a single parsed forum post.

---

## 🖥️ File Location

All exported files are saved to:
Desktop/output_yyyy-MM-dd_HH-mm-ss.csv


---

## ⚠️ Notes

- Ensure full page HTML is copied for best results
- Multiple pages are merged automatically
- Duplicate posts are removed during processing
- Long posts with quotes are preserved and cleaned automatically

---

## 🛠️ Features

- Multi-page HTML support
- Automatic deduplication
- Quote-aware parsing
- Clean CSV export
- Timestamped outputs
