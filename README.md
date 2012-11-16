TYPO3 Extension Generator
=========================

The **TYPO3 Extension Generator** is a tool to generate a TYPO3 extension from a laughably simple markup.

![](https://raw.github.com/oliversalzburg/typo3extensiongenerator/master/screenshot.png)

## Supports generating:
- MySQL Database Models
- TCA Configurations
- ExtBase Domain Models & Repositories
- ExtBase Controllers
- ExtBase SignalSlot Connections
- Frontend Plugins + FlexForm
- Services
- Language Resources
- TypoScript Resources

## Core Design Principles
- ### Only generates Fluid *stubs*; you're supposed to write your own templates.  
  The generated extension expects to have its generated stubs overwritten by your own templates. Your own templates are merged during the build process.

- ### Implement through Inheritance!  
  Your own classes (like controllers and repositories) always extend the generated wrappers. The wrappers contain all interfaces to the rest of the extension. These interface can be accessed in your classes through an injected reference to the wrapper.