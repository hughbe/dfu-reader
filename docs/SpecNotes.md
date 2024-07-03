# Specification Notes
- The [UM0391 User manual](./Resources/UM0391.pdf) specifies the DfuSe File Format Specification.
- The Specification was uploaded onto a forum in 2011 [here](https://community.st.com/t5/stm32-mcus-products/um0391-dfuse-file-format-specification/td-p/503561).
- This document lists errata from the specficiation UM0391 Specification.

## 2.1 DFU Prefix section
- The description states "The Prefix buffer is represented in Big Endian order." However, the Prefix buffer is represented in Little Endian order.
- The description for "DFUImageSize" should specify that it does not include the suffix (16 bytes).

## 2.3.2 Target Prefix
- The description states "The Target Prefix buffer is represented in Big Endian order." However, the Target Prefix buffer is represented in Little Endian order.
