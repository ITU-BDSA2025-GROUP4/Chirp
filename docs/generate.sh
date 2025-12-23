#!/bin/bash
# Helper script to generate the pdf of the report
# Require to have pandoc-crossref installed
# On arch install with "sudo pacman -S pandoc-crossref"
pandoc -F pandoc-crossref report.md -o 2025_itubdsa_group_04_report.pdf
