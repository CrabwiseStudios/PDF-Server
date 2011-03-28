# PDF Serve Quickstart Guide

## Overview

PDF Serve is designed to be a lightweight PDF web service that allows you to
generate PDFs from a template. It utilizes the [itextsharp](http://itextsharp.com/) library for creating
PDFs, [WCF-REST](http://msdn.microsoft.com/en-us/library/ms731082.aspx) for hosting the web service, and [MEF](http://mef.codeplex.com/) for creating and importing
templates.

## Getting Started

The best way to learn how to use PDFServe is by looking at the example hosted
on crabwisestudios.com. There you can find code snippets and try generating a
test PDF. This document should really only be needed if you'd like more details
on a certain aspect or if you're having trouble getting things working.

## The Web Service

### Hosting

You can host the web service by simply publishing the
Crabwise.PdfServe.Web.Services to your server. Alternatively, you can copy
over the binaries along with Global.asax and Web.config files. The web service
requires access to the filesystem in order to save PDFs. This may require you
to run the service under "Full Trust" in IIS.

### Configuring

The web service relies on two directories: Documents and Templates. The
documents directory will contain any generated PDFs. The "Templates"
directory will contain DLLs of your different PDF templates. By
default, both of these folders will be created at the web application's
root directory. For example, if you are hosting the web service at
http://crabwisestudios.com/PDFServe, the documents folder will be created at
http://crabwisestudios.com/PDFServer/Documents. The default settings specify
that a document should remain on the server for 1 week before being deleted.
The documents directory, templates directory, and document lifespan can all be
modified through settings in the service's Web.config.

### Usage

The web service has one method that automatically handles creating or
retrieving PDFs. This method takes in a template name which is specified in
the URL and a JSON array in the request's body. The JSON array can contain any
arbitrary key-value pairs which will be passed to the template. Upon successful
creation of a PDF, the method will return the PDF document. If the method fails
for any reason it will return an HTTP 500 error.

**NOTE:** *Due to limitations with WCF's JSON capabilities, the JSON data must
be defined as an array of objects which have a "Key" property and a "Value"
property. This is counter-intuitive to the standard way of creating a single
JSON object with key-value pairs.*

Details on this method can be found in the code's documentation and by going to
{web-service-url}/help/.

#### Example:

**URL:** http://{web-service-url}/{templateName} Body:**

        [ { "Key":"Name", "Value":"Bob" }, { "Key":"Date", "Value":"3/27/11" }
        ]

**DO NOT DO THIS:**

        // The following JSON won't work! { "Name":"Bob", "Date":"3/27/11" }

## Templates

### Creating

Making PDF templates is as simple as making a PDF with the itextsharp
library. The only PDFServe-specific requirement is that you implement
the IDocumentTemplate interface and adorn your class with the
`DocumentTemplateMetadata` attribute. Details on this interface can be found in
the library's reference documentation.

In general, templates have 2 main parts: 1) a name, and 2) code for writing
the PDF's contents. At runtime, your template will be given a dynamic object
which will have any template data passed in from the client. Names of templates
must be unique; for this reason, we recommend namespacing your templates. For
example, `com.crabwise.templates.helloworld`.

### Usage

After you have compiled any codecontaining your PDF templates, copy the DLL to
the Templates folder on your server. PDFServe will automatically detect and
load your templates as-needed at runtime.
